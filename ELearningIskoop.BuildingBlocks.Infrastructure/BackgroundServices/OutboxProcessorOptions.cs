using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.BuildingBlocks.Infrastructure.BackgroundServices
{
    public class OutboxProcessorOptions
    {
        public const string SectionName = "OutboxProcessor";

        /// <summary>
        /// Event processing interval in seconds
        /// </summary>
        public int ProcessingIntervalSeconds { get; set; } = 10;

        /// <summary>
        /// Maximum number of events to process in a single batch
        /// </summary>
        public int BatchSize { get; set; } = 50;

        /// <summary>
        /// Number of days to keep processed events before cleanup
        /// </summary>
        public int KeepProcessedEventsDays { get; set; } = 30;

        /// <summary>
        /// Maximum number of retry attempts for failed events
        /// </summary>
        public int MaxRetryAttempts { get; set; } = 3;

        /// <summary>
        /// Base delay in minutes for retry (exponential backoff)
        /// </summary>
        public int RetryBaseDelayMinutes { get; set; } = 2;

        /// <summary>
        /// Enable/disable automatic cleanup of old processed events
        /// </summary>
        public bool EnableAutoCleanup { get; set; } = true;

        /// <summary>
        /// Cleanup interval in hours
        /// </summary>
        public int CleanupIntervalHours { get; set; } = 24;

        /// <summary>
        /// Enable/disable processing (for maintenance purposes)
        /// </summary>
        public bool EnableProcessing { get; set; } = true;

        /// <summary>
        /// Process events by priority (high priority first)
        /// </summary>
        public bool PriorityProcessing { get; set; } = true;

        /// <summary>
        /// Maximum processing time per batch in minutes
        /// </summary>
        public int MaxProcessingTimeMinutes { get; set; } = 5;
    }
}
