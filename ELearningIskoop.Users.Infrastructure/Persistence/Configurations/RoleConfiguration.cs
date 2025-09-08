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
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles", "users");

            builder.HasKey(x => x.ObjectId);

            builder.Property(x => x.ObjectId).ValueGeneratedOnAdd();

            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.NormalizedName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(500);

            builder.HasMany(x => x.UserRoles).WithOne(ur => ur.Role).HasForeignKey(rp => rp.RoleId);
            // Permission relationship
            builder.HasMany(r => r.Permissions)
                .WithOne(p => p.Role)
                .HasForeignKey(p => p.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
