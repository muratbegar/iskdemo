using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.Shared.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ELearningIskoop.BuildingBlocks.Infrastructure.Outbox
{
    public class OutboxDbContext : DbContext
    {


        public OutboxDbContext(DbContextOptions<OutboxDbContext> options) : base(options)
        {
        }

        public DbSet<OutboxEvent> OutboxEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OutboxEvent>(entity =>
            {
                entity.ToTable("OutboxEvents", "shared");

                entity.HasKey(e => e.Id);

                // Properties
                entity.Property(e => e.EventType).IsRequired().HasMaxLength(255);
                entity.Property(e => e.EventData).IsRequired().HasColumnType("nvarchar(max)");
                entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.AggregateId).IsRequired().HasMaxLength(100);
                entity.Property(e => e.AggregateType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ModuleName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CorrelationId).HasMaxLength(100);
                entity.Property(e => e.UserId).HasMaxLength(50);
                entity.Property(e => e.LastError).HasMaxLength(2000);
                entity.Property(e => e.Priority).HasDefaultValue(0);
                entity.Property(e => e.MaxRetries).HasDefaultValue(3);

                // Indexes for performance

                entity.HasIndex(e => new { e.ModuleName, e.AggregateType, e.AggregateId })
                    .HasDatabaseName("IX_OutboxEvents_Module");

                entity.HasIndex(e => new { e.Processed, e.ProcessedAt })
                    .HasDatabaseName("IX_OutboxEvents_Cleanup");

                entity.HasIndex(e => e.CorrelationId)
                    .HasDatabaseName("IX_OutboxEvents_Correlation");

                entity.HasIndex(e => new { e.UserId, e.CreatedAt })
                    .HasDatabaseName("IX_OutboxEvents_User");
            });

            base.OnModelCreating(modelBuilder);
        }
    }

}
