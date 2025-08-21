using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Domain;

namespace ELearningIskoop.Users.Domain.Entities
{
    public class LoginHistory : BaseEntity
    {
        public int UserId{ get; set; }
        public DateTime LoginTime{ get; set; }
        public string IpAddress{ get; set; }
        public bool IsSuccessful{ get; set; }
    }
}
