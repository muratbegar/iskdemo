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
    public class UserLoginConfiguration : IEntityTypeConfiguration<UserLogin>
    {
        public void Configure(EntityTypeBuilder<UserLogin> builder)
        {
            builder.ToTable("UserLogins");
            builder.HasKey(ul => ul.ObjectId);
            builder.Property(ul => ul.ObjectId).ValueGeneratedOnAdd();

            builder.Property(ul => ul.IpAddress)
                .IsRequired()
                .HasMaxLength(45);

            builder.Property(ul => ul.UserAgent)
                .HasMaxLength(500);

            builder.HasIndex(ul => ul.UserId)
                .HasDatabaseName("IX_UserLogins_UserId");

            builder.HasIndex(ul => ul.LoginAt)
                .HasDatabaseName("IX_UserLogins_LoginAt");
        }
    }

}
