using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELearningIskoop.Users.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Mg_User_005 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "users",
                table: "UserEmailVerifications");

            migrationBuilder.AddColumn<string>(
                name: "UserMail",
                schema: "users",
                table: "UserEmailVerifications",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserMail",
                schema: "users",
                table: "UserEmailVerifications");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                schema: "users",
                table: "UserEmailVerifications",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
