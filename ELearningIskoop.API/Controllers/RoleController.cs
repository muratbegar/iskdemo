using ELearningIskoop.API.Middleware;
using ELearningIskoop.API.Models.Response;
using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Users.Application.Commands.CreateRole;
using ELearningIskoop.Users.Application.Commands.DeleteRole;
using ELearningIskoop.Users.Application.Commands.UpdateRole;
using ELearningIskoop.Users.Application.DTOs;
using ELearningIskoop.Users.Application.DTOs.Role;
using ELearningIskoop.Users.Application.Queries.GetAllRoles;
using ELearningIskoop.Users.Application.Queries.GetRoleById;
using ELearningIskoop.Users.Application.Queries.GetUserById;
using ELearningIskoop.Users.Application.Queries.SearchRoles;
using ELearningIskoop.Users.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ELearningIskoop.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class RoleController : ControllerBase
    {

        private readonly ILogger<RoleController> _logger;
        private readonly IMediator _mediator;

        public RoleController(IMediator mediator, ILogger<RoleController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }



        //get user by me myprofile
        [HttpPost("create-role")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Create Role",
            Description = "Rol Oluşturma",
            OperationId = "User.Role.Create",
            Tags = new[] { "Profile" }
        )]
        [SwaggerResponse(201, "Role created successfully", typeof(ApiResponse<Role>))]
        [SwaggerResponse(400, "Invalid request data", typeof(ErrorResponse))]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand role)
        {
            try
            {
                var roleCmd = await _mediator.Send(new CreateRoleCommand()
                {
                    RoleName = role.RoleName,
                    Description = role.Description,
                    CreatedAt = DateTime.UtcNow,
                    RequestedBy = 0
                });

                return Ok(new ApiResponse<Role>
                {
                    Success = true,
                    Data = roleCmd.Value
                });
            }
            catch (ValidationException ex)
            {
                // FluentValidation errors
                var errors = ex.ValidationResult.ErrorMessage.ToList();
                return BadRequest(new { Errors = errors, Message = "Validation failed" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred" });
            }
        }


        //get user by me myprofile
        [HttpPut("update-role")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Update Role",
            Description = "Rol Güncelleme",
            OperationId = "User.Role.Update",
            Tags = new[] { "Profile" }
        )]
        [SwaggerResponse(201, "Role updated successfully", typeof(ApiResponse<Role>))]
        [SwaggerResponse(400, "Invalid request data", typeof(ErrorResponse))]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleCommand command)
        {
            try
            {
                command.RoleId = id; // Route'dan ID'yi command'a aktar
                var response = await _mediator.Send(command);
                return response.IsSuccess ? Ok(response) : BadRequest(response);
            }
            catch (ValidationException ex)
            {
                // FluentValidation errors
                var errors = ex.ValidationResult.ErrorMessage.ToList();
                return BadRequest(new { Errors = errors, Message = "Validation failed" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred" });
            }
        }



        [HttpDelete("delete-role")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Delete Role",
            Description = "Rol Silme",
            OperationId = "User.Role.Delete",
            Tags = new[] { "Profile" }
        )]
        [SwaggerResponse(201, "Role deleted successfully", typeof(ApiResponse<Role>))]
        [SwaggerResponse(400, "Invalid request data", typeof(ErrorResponse))]
        public async Task<IActionResult> DeleteRole(int id)
        {
            try
            {
                var command = new DeleteRoleCommand { RoleId = id };
                var response = await _mediator.Send(command);
                return response.IsSuccess ? Ok(response) : BadRequest(response);
            }
            catch (ValidationException ex)
            {
                // FluentValidation errors
                var errors = ex.ValidationResult.ErrorMessage.ToList();
                return BadRequest(new { Errors = errors, Message = "Validation failed" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred" });
            }
        }

        /// <summary>
        /// Get a role by ID
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <returns>Role details</returns>
        [HttpGet("get-role")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Get Role",
            Description = "Rol Getir",
            OperationId = "User.Role.Get",
            Tags = new[] { "Profile" }
        )]
        [SwaggerResponse(201, "Role get successfully", typeof(ApiResponse<RoleDto>))]
        [SwaggerResponse(400, "Invalid request data", typeof(ErrorResponse))]
        public async Task<IActionResult> GetRoleById(int id)
        {
            try
            {
                var query = new GetRoleIdQuery(id);
                var role = await _mediator.Send(query);

                return role != null
                    ? Ok(new ApiResponse<RoleDto> { Success = true, Data = role })
                    : NotFound(new ErrorResponse { Detail = "Role not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred" });
            }


        }


        /// <summary>
        /// Get all roles with pagination
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 100)</param>
        /// <param name="sortBy">Sort field: Name or CreatedAt (default: Name)</param>
        /// <param name="sortDescending">Sort direction (default: false)</param>
        /// <returns>Paginated list of roles</returns>
        [HttpGet]
        [Authorize]
        [SwaggerOperation(
            Summary = "Get Role All",
            Description = "Rol Hepsi Getir",
            OperationId = "User.Role.GetAll",
            Tags = new[] { "Profile" }
        )]
        [SwaggerResponse(201, "Role get successfully", typeof(ApiResponse<RoleDto>))]
        [SwaggerResponse(400, "Invalid request data", typeof(ErrorResponse))]
        public async Task<IActionResult> GetAllRoles(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "Name",
            [FromQuery] bool sortDescending = false)
        {
            try
            {
                var query = new GetAllRolesQuery(page, pageSize, sortBy, sortDescending);
                var result = await _mediator.Send(query);

                // Response headers ile pagination bilgisi
                Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
                Response.Headers.Add("X-Page", result.PageNumber.ToString());
                Response.Headers.Add("X-Page-Size", result.PageSize.ToString());
                Response.Headers.Add("X-Total-Pages", result.TotalPages.ToString());

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred" });
            }
        }







        /// <summary>
        /// Search roles by name or description
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 100)</param>
        /// <param name="sortBy">Sort field: Name or CreatedAt (default: Name)</param>
        /// <param name="sortDescending">Sort direction (default: false)</param>
        /// <returns>Paginated search results</returns>
        [HttpGet("search")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Get Role All",
            Description = "Rol Hepsi Getir",
            OperationId = "User.Role.GetAll",
            Tags = new[] { "Profile" }
        )]
        [SwaggerResponse(201, "Role get successfully", typeof(ApiResponse<RoleDto>))]
        [SwaggerResponse(400, "Invalid request data", typeof(ErrorResponse))]
        public async Task<IActionResult> SearchRoles(
            [FromQuery] string searchTerm,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "Name",
            [FromQuery] bool sortDescending = false)
        {
            try
            {
                var query = new SearchRolesQuery(searchTerm, page, pageSize)
                {
                    SortBy = sortBy,
                    SortDescending = sortDescending
                };

                var result = await _mediator.Send(query);

                // Response headers ile pagination bilgisi
                Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
                Response.Headers.Add("X-Page", result.Page.ToString());
                Response.Headers.Add("X-Page-Size", result.PageSize.ToString());
                Response.Headers.Add("X-Total-Pages", result.TotalPages.ToString());

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred" });
            }
        }

    }
}
