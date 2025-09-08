using ELearningIskoop.BuildingBlocks.Domain;
using System.Net;
using System.Text.Json;

namespace ELearningIskoop.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }
        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = exception switch
            {
                EntityNotFoundException ex => new ErrorResponse
                {
                    Title = "Not Found",
                    Status = (int)HttpStatusCode.NotFound,
                    Detail = ex.Message,
                    ErrorCode = ex.ErrorCode
                },
                BusinessRuleViolationException ex => new ErrorResponse
                {
                    Title = "Business Rule Violation",
                    Status = (int)HttpStatusCode.BadRequest,
                    Detail = ex.Message,
                    ErrorCode = ex.ErrorCode
                },
                DomainException ex => new ErrorResponse
                {
                    Title = "Domain Error",
                    Status = (int)HttpStatusCode.BadRequest,
                    Detail = ex.Message,
                    ErrorCode = ex.ErrorCode
                },
                UnauthorizedException ex => new ErrorResponse
                {
                    Title = "Unauthorized",
                    Status = (int)HttpStatusCode.Unauthorized,
                    Detail = ex.Message,
                    ErrorCode = "UNAUTHORIZED"
                },
                ValidationException ex => new ErrorResponse
                {
                    Title = "Validation Failed",
                    Status = (int)HttpStatusCode.BadRequest,
                    Detail = ex.Message,
                    ErrorCode = "VALIDATION_ERROR"
                },
                _ => new ErrorResponse
                {
                    Title = "Internal Server Error",
                    Status = (int)HttpStatusCode.InternalServerError,
                    Detail = "An error occurred while processing your request",
                    ErrorCode = "INTERNAL_ERROR"
                }
            };

            context.Response.StatusCode = response.Status;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var jsonResponse = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(jsonResponse);
        }

        public class UnauthorizedException : Exception
        {
            public UnauthorizedException(string message) : base(message) { }
        }

        public class ValidationException : Exception
        {
            public ValidationException(string message) : base(message) { }
        }
    }
}
