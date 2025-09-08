using ELearningIskoop.Shared.Domain.ValueObjects;
using ELearningIskoop.Users.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Users.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(x => x.ObjectId);

            // Properties
            builder.Property(x => x.ObjectId)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Username)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.SecurityStamp)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(x => x.ProfilePictureUrl)
                .HasMaxLength(500);

            builder.Property(x => x.Bio)
                .HasMaxLength(1000);

            builder.Property(x => x.PhoneNumber)
                .HasMaxLength(20);

            // Security & Login Properties
            builder.Property(x => x.FailedLoginAttempts)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(x => x.LastLoginAt);

            builder.Property(x => x.LockedUntil);

            builder.Property(x => x.EmailVerifiedAt);

            // Computed properties için ignore
            builder.Ignore(x => x.IsLocked);
            builder.Ignore(x => x.IsMailVerified);

            // Email Value Object konfigürasyonu - DÜZELTME
            builder.Property(x => x.Email)
                .HasConversion(
                    email => email.Value,                    // Entity -> Database
                    value => new Email(value))               // Database -> Entity
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(255);

            // Email için unique index
            builder.HasIndex("Email")
                .IsUnique()
                .HasDatabaseName("IX_Users_Email");

            // PersonName Value Object konfigürasyonu
            builder.OwnsOne(u => u.Name, nameBuilder =>
            {
                nameBuilder.Property(n => n.FirstName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("FirstName");

                nameBuilder.Property(n => n.LastName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("LastName");

            });

            // HashedPassword Value Object konfigürasyonu
            builder.OwnsOne(u => u.Password, passwordBuilder =>
            {
                passwordBuilder.Property(p => p.Hash)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("PasswordHash");

                passwordBuilder.Property(p => p.Salt)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("PasswordSalt");
            });

            // Navigation Properties - RefreshTokens
            builder.HasMany(u => u.RefreshTokens)
                .WithOne()
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Cascade);

            // Navigation Properties - UserRoles
            builder.HasMany(u => u.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Navigation Properties - LoginHistory
            builder.HasMany(u => u.LoginHistory)
                .WithOne(ul => ul.User)
                .HasForeignKey(ul => ul.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(u => u.Username)
                .IsUnique()
                .HasDatabaseName("IX_Users_Username");

            builder.HasIndex(u => u.Status)
                .HasDatabaseName("IX_Users_Status");

            // Audit fields
            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt);

            builder.Property(x => x.CreatedBy);

            builder.Property(x => x.UpdatedBy);

            builder.Property(x => x.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.DeletedAt);

            builder.Property(x => x.DeletedBy);

        }
    }

}
