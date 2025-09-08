using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ELearningIskoop.BuildingBlocks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Mg_Outbox_001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "shared");

            migrationBuilder.CreateTable(
                name: "OutboxEvents",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    EventData = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Processed = table.Column<bool>(type: "boolean", nullable: false),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    MaxRetries = table.Column<int>(type: "integer", nullable: false, defaultValue: 3),
                    LastError = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    NextRetryAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AggregateId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AggregateType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ModuleName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CorrelationId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxEvents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxEvents_Cleanup",
                schema: "shared",
                table: "OutboxEvents",
                columns: new[] { "Processed", "ProcessedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxEvents_Correlation",
                schema: "shared",
                table: "OutboxEvents",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxEvents_Module",
                schema: "shared",
                table: "OutboxEvents",
                columns: new[] { "ModuleName", "AggregateType", "AggregateId" });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxEvents_User",
                schema: "shared",
                table: "OutboxEvents",
                columns: new[] { "UserId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutboxEvents",
                schema: "shared");
        }
    }
}
