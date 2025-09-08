using ELearningIskoop.API.Models.Response;
using ELearningIskoop.BuildingBlocks.Domain;
using ELearningIskoop.Courses.Application.DTOs.Common;
using ELearningIskoop.Users.Application.Commands.UpdateUserProfile;
using ELearningIskoop.Users.Application.DTOs;
using ELearningIskoop.Users.Application.Queries.GetMyProfile;
using ELearningIskoop.Users.Application.Queries.GetUserById;
using ELearningIskoop.Users.Application.Queries.SearchUser;
using ELearningIskoop.Users.Domain.Aggregates;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using ELearningIskoop.Shared.Domain.ValueObjects;
using ELearningIskoop.Users.Application.Commands.ChangePassword;

namespace ELearningIskoop.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IMediator _mediator;

        public UserController(IMediator mediator, ILogger<UserController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


        //get user by me myprofile
        [HttpPost("me")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Profilim",
            Description = "Profil Bilgileri ",
            OperationId = "User.Profile",
            Tags = new[] { "Profile" }
        )]
        [SwaggerResponse(201, "User created successfully", typeof(ApiResponse<User>))]
        [SwaggerResponse(400, "Invalid request data", typeof(ErrorResponse))]
        public async Task<IActionResult> MyProfile()
        {

            var profile = await _mediator.Send(new GetMyProfileQuery
            {
                RequestedBy = 0
            });

            return Ok(new ApiResponse<UserDTO>
            {
                Success = true,
                Data = profile
            });
        }


        //get user by me myprofile
        [HttpPost("get-user-by-id")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Profilim",
            Description = "Profil Bilgileri ",
            OperationId = "User.Profile.GetById",
            Tags = new[] { "Profile" }
        )]
        [SwaggerResponse(201, "User created successfully", typeof(ApiResponse<User>))]
        [SwaggerResponse(400, "Invalid request data", typeof(ErrorResponse))]
        public async Task<IActionResult> GetUserById(int UserId)
        {

            var profile = await _mediator.Send(new GetUserByIdQuery()
            {
                UserId = UserId,
                RequestedBy = UserId
            });

            return Ok(new ApiResponse<UserDTO>
            {
                Success = true,
                Data = profile
            });
        }



        [HttpPut("update-profile")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Profil Güncelleme",
            Description = "Profil Güncelleme Bilgileri ",
            OperationId = "User.Profile.Update",
            Tags = new[] { "Profile" }
        )]
        [SwaggerResponse(201, "User updated successfully", typeof(ApiResponse<User>))]
        [SwaggerResponse(400, "Invalid request data", typeof(ErrorResponse))]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileCommand updateUserDto)
        {
            var result = await _mediator.Send(updateUserDto);

            _logger.LogInformation("Course updated successfully: {UserId}", updateUserDto.UserId);
            return Ok(new ApiResponse<UserDTO>
            {
                Success = true,
                Message = "User updated successfully",
                Data = result
            });

        }

        //[HttpPost("update-profile-picture")]
        //[Authorize]
        //[SwaggerOperation(
        //    Summary = "Profil Fotoğrafı Güncelleme",
        //    Description = "Profil Güncelleme Bilgileri ",
        //    OperationId = "User.Profile.Picture",
        //    Tags = new[] { "Profile" }
        //)]
        //[ProducesResponseType(typeof(PagedResponse<UserDTO>), 200)]
        //[ProducesResponseType(400)]
        //public async Task<IActionResult> UploadProfilePicture([FromForm] IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest("Dosya boş olamaz.");

        //    // Kullanıcıyı JWT veya Context üzerinden al
        //    var profile = await _mediator.Send(new GetMyProfileQuery
        //    {
        //        RequestedBy = 0
        //    });
        //    if(profile == null)
        //        return NotFound("Kullanıcı bulunamadı.");

        //   var userId = profile.Id;
        //    //// Dosyayı kaydetmek için bir yol belirle
        //    //var uploadsFolder = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        //    //if (!Directory.Exists(uploadsFolder))
        //    //{
        //    //    Directory.CreateDirectory(uploadsFolder);
        //    //}
        //    //var filePath = System.IO.Path.Combine(uploadsFolder, $"{Guid.NewGuid()}_{file.FileName}");
        //    //using (var stream = new FileStream(filePath, FileMode.Create))
        //    //{
        //    //    await file.CopyToAsync(stream);
        //    //}
        //    // Dosya URL'sini oluştur (örneğin, wwwroot/uploads/filename)
        //    var fileUrl = "test resim";
        //    // Kullanıcının profil resmini güncelle
        //    var updateCommand = new UpdateUserProfileCommand
        //    {
        //        UserId = userId,
        //        ProfilePictureUrl = fileUrl
        //    };
        //    var result = await _mediator.Send(updateCommand);
        //    return Ok(new ApiResponse<UserDTO>
        //    {
        //        Success = true,
        //        Message = "Profil resmi başarıyla güncellendi.",
        //        Data = result
        //    });
        //}



        [HttpGet("search-user")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Profil Arama",
            Description = "Profil Güncelleme Bilgileri ",
            OperationId = "User.Profile.Search.User",
            Tags = new[] { "Profile" }
        )]
        [ProducesResponseType(typeof(PagedResponse<UserDTO>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> SearchUsers(
            [FromQuery] string? searchTerm,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool sortDescending = false)
        {
            var query = new SearchUserQuery()
            {
                SearchTerm = searchTerm,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDescending = sortDescending
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("change-password")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Şifre değiştirme",
            Description = "Şifre Değiştirme",
            OperationId = "User.Password",
            Tags = new[] { "Profile" }
        )]
        [SwaggerResponse(201, "User change password successfully", typeof(ApiResponse<User>))]
        [SwaggerResponse(400, "Invalid request data", typeof(ErrorResponse))]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand changePassword)
        {
             await _mediator.Send(changePassword);

             return Ok(new ApiResponse<UserDTO>
             {
                 Success = true,
                 Message = "User updated successfully",
                 Data = null
             });

        }

    }
}
