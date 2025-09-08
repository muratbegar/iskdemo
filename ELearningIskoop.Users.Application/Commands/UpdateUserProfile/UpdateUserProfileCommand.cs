using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Application.CQRS;
using ELearningIskoop.Users.Application.DTOs;

namespace ELearningIskoop.Users.Application.Commands.UpdateUserProfile
{
    public record UpdateUserProfileCommand : BaseCommand<UserDTO>
    {
    public int UserId { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Bio { get; init; }
    public string? PhoneNumber { get; init; }
    public string? ProfilePictureUrl { get; init; }
    }
}
