using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Users.Domain.Entities;

namespace ELearningIskoop.Users.Infrastructure.Persistence.Configurations
{
    public class RolePermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("RolePermissions");
            builder.HasKey(rp => rp.ObjectId);
            builder.Property(rp => rp.ObjectId).ValueGeneratedOnAdd();

            builder.Property(rp => rp.Permissions)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasIndex(rp => new { rp.RoleId, rp.Permissions })
                .IsUnique()
                .HasDatabaseName("IX_RolePermissions_RoleId_Permission");
        }
    }

}
