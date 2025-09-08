using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Users.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ELearningIskoop.Users.Infrastructure.Persistence.Configurations
{
    public class PasswordResetConfiguration : IEntityTypeConfiguration<PasswordResetToken>
    {
        public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
        {
            builder.ToTable("PasswordResetTokens");
            builder.HasKey(pr => pr.ObjectId);
            builder.Property(pr => pr.UserId)
                .IsRequired();
            builder.Property(pr => pr.Token)
                .IsRequired()
                .HasMaxLength(6);
            builder.Property(x => x.ExpiresAt)
                .IsRequired();

            builder.Property(x => x.IsUsed)
                .IsRequired();

        }
    }
}
