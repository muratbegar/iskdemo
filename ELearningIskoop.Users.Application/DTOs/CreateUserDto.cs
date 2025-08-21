using ELearningIskoop.Shared.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Application.DTOs
{
    public record CreateUserDto(
        Email Email,
        PersonName Name,
        string Password);

}
