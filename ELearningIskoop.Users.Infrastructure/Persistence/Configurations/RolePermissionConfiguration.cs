using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Users.Domain.Entities;
using ELearningIskoop.Users.Domain.ValueObjects.Permissions;
using Action = ELearningIskoop.Users.Domain.ValueObjects.Permissions.Action;


namespace ELearningIskoop.Users.Infrastructure.Persistence.Configurations
{
    public class RolePermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("Permissions", "users");

            // Primary Key
            builder.HasKey(p => p.ObjectId);
            // Properties
            builder.Property(p => p.RoleId)
                .IsRequired();

            builder.Property(p => p.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(p => p.UpdatedAt)
                .IsRequired(false);

            builder.OwnsOne(p => p.PermissionValue, pv =>
            {
                pv.Property(p => p.Resource)
                    .HasConversion(
                        r => r.Value,
                        v => Resource.From(v))
                    .HasColumnName("Resource")
                    .HasMaxLength(50)
                    .IsRequired();

                pv.Property(p => p.Action)
                    .HasConversion(
                        a => a.Value,
                        v => Action.From(v))
                    .HasColumnName("Action")
                    .HasMaxLength(20)
                    .IsRequired();

                pv.Property(p => p.Scope)
                    .HasConversion(
                        s => s.Value,
                        v => Scope.From(v))
                    .HasColumnName("Scope")
                    .HasMaxLength(20)
                    .IsRequired();
            });

            // Foreign Key
            builder.HasOne(p => p.Role)
                .WithMany() // Role entity'sinde Permissions collection yoksa
                .HasForeignKey(p => p.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(p => p.RoleId)
                .HasDatabaseName("IX_Permissions_RoleId");

          

            builder.HasIndex(p => p.IsActive)
                .HasDatabaseName("IX_Permissions_IsActive");
        }
    }

}
