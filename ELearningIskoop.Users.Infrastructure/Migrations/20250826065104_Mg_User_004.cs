using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELearningIskoop.Users.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Mg_User_004 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserEmailVerifications_Users_UserId",
                schema: "users",
                table: "UserEmailVerifications");

            migrationBuilder.DropIndex(
                name: "IX_UserEmailVerifications_UserId",
                schema: "users",
                table: "UserEmailVerifications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserEmailVerifications_UserId",
                schema: "users",
                table: "UserEmailVerifications",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserEmailVerifications_Users_UserId",
                schema: "users",
                table: "UserEmailVerifications",
                column: "UserId",
                principalSchema: "users",
                principalTable: "Users",
                principalColumn: "ObjectId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
