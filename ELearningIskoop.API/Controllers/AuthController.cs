using ELearningIskoop.API.Middleware;
using ELearningIskoop.API.Models.Requests;
using ELearningIskoop.API.Models.Response;
using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Shared.Domain.ValueObjects;
using ELearningIskoop.Users.Application.Commands.ForgotPassword;
using ELearningIskoop.Users.Application.Commands.LoginUser;
using ELearningIskoop.Users.Application.Commands.RegisterUser;
using ELearningIskoop.Users.Application.Commands.ResetPassword;
using ELearningIskoop.Users.Application.Commands.VerifyEmail;
using ELearningIskoop.Users.Application.DTOs;
using ELearningIskoop.Users.Application.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using static ELearningIskoop.API.Middleware.ExceptionHandlingMiddleware;
using ErrorResponse = ELearningIskoop.BuildingBlocks.Domain.ErrorResponse;

namespace ELearningIskoop.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IMediator mediator,
            ITokenService tokenService,
            ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _tokenService = tokenService;
            _logger = logger;
        }


        // Register a new user
        [HttpPost("register")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Register new user",
            Description = "Creates a new user account and sends email verification",
            OperationId = "Auth.Register",
            Tags = new[] { "Authentication" }
        )]
        [SwaggerResponse(201, "User created successfully", typeof(ApiResponse<UserDTO>))]
        [SwaggerResponse(400, "Invalid request data", typeof(ErrorResponse))]
        [SwaggerResponse(409, "Email already exists", typeof(ErrorResponse))]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var command = new RegisterUserCommand
                {
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Password = request.Password,
                    ConfirmPassword = request.ConfirmPassword,
                    RequestedBy = null,
                    TrackingId = Guid.NewGuid().ToString()
                };
                var result = await _mediator.Send(command);
                _logger.LogInformation("New user registered: {Email}", request.Email);

                return Created($"/api/v1/users/{result.Id}", new ApiResponse<UserDTO>
                {
                    Success = true,
                    Message = "User registered successfully. Please check your email for verification.",
                    Data = result
                });
            }
            catch (ApplicationException ex)
            {
                _logger.LogWarning("Registration failed: {Error}", ex.Message);
                return Conflict(new ErrorResponse
                {
                    Title = "Registration Failed",
                    Status = 409,
                    Detail = ex.Message,
                    ErrorCode = "REGISTRATION_FAILED"
                });
            }
            catch (ExceptionHandlingMiddleware.ValidationException ex)
            {
                return BadRequest(new ErrorResponse
                {
                    Title = "Validation Failed",
                    Status = 400,
                    Detail = ex.Message,
                    ErrorCode = "VALIDATION_ERROR"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during registration");
                return StatusCode(500, new ErrorResponse
                {
                    Title = "Internal Server Error",
                    Status = 500,
                    Detail = "An unexpected error occurred while processing your request."
                });
            }
        }



        [HttpPost("login")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "User login",
            Description = "Authenticates user and returns JWT token",
            OperationId = "Auth.Login",
            Tags = new[] { "Authentication" }
        )]
        [SwaggerResponse(200, "Login successful", typeof(ApiResponse<LoginResponse>))]
        [SwaggerResponse(401, "Invalid credentials", typeof(ErrorResponse))]
        [SwaggerResponse(423, "Account locked", typeof(ErrorResponse))]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                var userAgent = Request.Headers["User-Agent"].ToString();

                var command = new LoginUserCommand
                {
                    Email = request.Email,
                    Password = request.Password,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    TrackingId = Guid.NewGuid().ToString()
                };

                var result = await _mediator.Send(command);

                // Set refresh token in HTTP-only cookie
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7)
                };

                _logger.LogInformation("User logged in: {Email}", request.Email);

                return Ok(new ApiResponse<LoginResponse>
                {
                    Success = true,
                    Message = "Login successful",
                    Data = new LoginResponse
                    {
                        AccessToken = result.AccessToken
                    }
                });
            }
            catch (UnauthorizedException ex)
            {
                _logger.LogWarning("Login failed for {Email}: {Error}", request.Email, ex.Message);
                return Unauthorized(new ErrorResponse
                {
                    Title = "Authentication Failed",
                    Status = 401,
                    Detail = ex.Message,
                    ErrorCode = "INVALID_CREDENTIALS"
                });
            }
            catch (BusinessRuleViolationException ex) when (ex.Message.Contains("locked"))
            {
                return StatusCode(423, new ErrorResponse
                {
                    Title = "Account Locked",
                    Status = 423,
                    Detail = ex.Message,
                    ErrorCode = "ACCOUNT_LOCKED"
                });
            }
        }


        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Refresh access token",
            Description = "Uses refresh token from cookie to generate new access token",
            OperationId = "Auth.RefreshToken",
            Tags = new[] { "Authentication" }
        )]
        [SwaggerResponse(200, "Token refreshed", typeof(ApiResponse<RefreshTokenResponse>))]
        [SwaggerResponse(401, "Invalid refresh token", typeof(ErrorResponse))]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new ErrorResponse
                {
                    Title = "Refresh Token Missing",
                    Status = 401,
                    Detail = "Refresh token not found",
                    ErrorCode = "REFRESH_TOKEN_MISSING"
                });
            }

            try
            {
                var result = await _tokenService.RefreshTokenAsync(refreshToken);
                // Update refresh token in HTTP-only cookie
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7)
                };
                Response.Cookies.Append("refreshToken", result.NewRefreshToken, cookieOptions);
                _logger.LogInformation("Refresh token used to generate new access token");
                return Ok(new ApiResponse<RefreshTokenResponse>
                {
                    Success = true,
                    Message = "Token refreshed successfully",
                    Data = new RefreshTokenResponse
                    {
                        AccessToken = result.AccessToken!,
                        ExpiresAt = result.ExpiresAt!.Value
                    }
                });
            }
            catch (UnauthorizedException ex)
            {
                _logger.LogWarning("Refresh token failed: {Error}", ex.Message);
                return Unauthorized(new ErrorResponse
                {
                    Title = "Invalid Refresh Token",
                    Status = 401,
                    Detail = ex.Message,
                    ErrorCode = "INVALID_REFRESH_TOKEN"
                });
            }
        }



        [HttpPost("Logout")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Logout user",
            Description = "Revokes refresh token and clears authentication",
            OperationId = "Auth.Logout",
            Tags = new[] { "Authentication" }
        )]
        [SwaggerResponse(200, "Logout successful", typeof(ApiResponse<object>))]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (!string.IsNullOrEmpty(refreshToken))
            {
                await _tokenService.RevokeTokenAsync(refreshToken, "User logout");
                Response.Cookies.Delete("refreshToken");
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("User logged out: {UserId}", userId);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Logout successful"
            });
        }


        [HttpPost("verify-email")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Verify email address",
            Description = "Confirms user email address using verification token",
            OperationId = "Auth.VerifyEmail",
            Tags = new[] { "Authentication" }
        )]
        [SwaggerResponse(200, "Email verified", typeof(ApiResponse<object>))]
        [SwaggerResponse(400, "Invalid token", typeof(ErrorResponse))]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            try
            {
                var command = new VerifyEmailCommand
                {
                    UserId = request.UserId,
                    VerificationToken = request.Token,
                };

                await _mediator.Send(command);

                _logger.LogInformation("Email verified for user: {UserId}", request.UserId);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Email verified successfully. You can now login."
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Email verification failed: {Error}", ex.Message);
                return BadRequest(new ErrorResponse
                {
                    Title = "Verification Failed",
                    Status = 400,
                    Detail = ex.Message,
                    ErrorCode = "VERIFICATION_FAILED"
                });
            }
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Request password reset",
            Description = "Sends password reset link to user's email",
            OperationId = "Auth.ForgotPassword",
            Tags = new[] { "Authentication" }
        )]
        [SwaggerResponse(200, "Reset email sent", typeof(ApiResponse<object>))]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                var command = new ForgotPasswordCommand()
                {
                    Email = request.Email
                };

                await _mediator.Send(command);

                _logger.LogInformation("Email verified for user: {UserId}", request.Email);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Email verified successfully. You can now login."
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Email verification failed: {Error}", ex.Message);
                return BadRequest(new ErrorResponse
                {
                    Title = "Verification Failed",
                    Status = 400,
                    Detail = ex.Message,
                    ErrorCode = "VERIFICATION_FAILED"
                });
            }
        }


        [HttpPost("reset-password")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Reset user password",
            Description = "Resets user's password using the reset token",
            OperationId = "Auth.ResetPassword",
            Tags = new[] { "Authentication" }
        )]
        [SwaggerResponse(200, "Password reset successfully", typeof(ApiResponse<object>))]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                var command = new ResetPasswordCommand(new Email(request.Email),request.Token, request.NewPassword);
                await _mediator.Send(command);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Password has been reset successfully."
                });
            }
            catch (BusinessRuleViolationException ex)
            {
                return BadRequest(new ErrorResponse
                {
                    Title = "Reset Failed",
                    Status = 400,
                    Detail = ex.Message,
                    ErrorCode = "RESET_PASSWORD_FAILED"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during password reset");
                return StatusCode(500, new ErrorResponse
                {
                    Title = "Server Error",
                    Status = 500,
                    Detail = ex.Message,
                    ErrorCode = "SERVER_ERROR"
                });
            }
        }

    }
}
