using ELearningIskoop.Users.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Users.Domain.Aggregates;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ELearningIskoop.Users.Infrastructure.Persistence.Configurations
{
    public class UserVerificationConfiguration : IEntityTypeConfiguration<UserEmailVerification>
    {
        public void Configure(EntityTypeBuilder<UserEmailVerification> builder)
        {
            builder.ToTable("UserEmailVerifications");

            builder.HasKey(uv => uv.ObjectId);

            builder.Property(uv => uv.UserMail)
                .IsRequired();

            builder.Property(uv => uv.Code).IsRequired().HasMaxLength(6);
            builder.Property(x => x.ExpiresAt)
                .IsRequired();

            builder.Property(x => x.IsUsed)
                .IsRequired();

           

        }
    }
}
