using ELearningIskoop.Users.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Infrastructure.Persistence.Configurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("UserRoles");
            builder.HasKey(x => x.ObjectId);
            builder.Property(x => x.ObjectId).ValueGeneratedOnAdd();
            builder.HasIndex(ur => new { ur.UserId, ur.RoleId })
                .IsUnique()
                .HasDatabaseName("IX_UserRoles_UserId_RoleId");
        }
    }

}
