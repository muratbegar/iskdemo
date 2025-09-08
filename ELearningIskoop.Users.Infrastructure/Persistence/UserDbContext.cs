using ELearningIskoop.Users.Domain.Aggregates;
using ELearningIskoop.Users.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Users.Infrastructure.Persistence.Configurations;

namespace ELearningIskoop.Users.Infrastructure.Persistence
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; } = null;
        public DbSet<Role> Roles { get; set; } = null;

        public DbSet<UserRole> UserRoles { get; set; } = null;

        public DbSet<UserLogin> UsersLogin { get; set; } = null;

        public DbSet<Permission> RolePermissions { get; set; } = null;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null;
        public DbSet<UserEmailVerification> UserEmailVerifications { get; set; } = null;

        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Schema
            modelBuilder.HasDefaultSchema("users");

            // Apply configurations
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserLoginConfiguration());
            modelBuilder.ApplyConfiguration(new RolePermissionConfiguration());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
            modelBuilder.ApplyConfiguration(new UserVerificationConfiguration());
            modelBuilder.ApplyConfiguration(new PasswordResetConfiguration());

            modelBuilder.Entity<User>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Role>().HasQueryFilter(x => !x.IsDeleted);
        }
    }

}
