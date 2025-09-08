using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELearningIskoop.Users.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Mg_User_008 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_Roles_RoleId",
                schema: "users",
                table: "RolePermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermissions",
                schema: "users",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_RoleId_Permission",
                schema: "users",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "Permissions",
                schema: "users",
                table: "RolePermissions");

            migrationBuilder.RenameTable(
                name: "RolePermissions",
                schema: "users",
                newName: "Permissions",
                newSchema: "users");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "users",
                table: "Permissions",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "users",
                table: "Permissions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<string>(
                name: "Action",
                schema: "users",
                table: "Permissions",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Resource",
                schema: "users",
                table: "Permissions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RoleObjectId",
                schema: "users",
                table: "Permissions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Scope",
                schema: "users",
                table: "Permissions",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Permissions",
                schema: "users",
                table: "Permissions",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_IsActive",
                schema: "users",
                table: "Permissions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_RoleId",
                schema: "users",
                table: "Permissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_RoleObjectId",
                schema: "users",
                table: "Permissions",
                column: "RoleObjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Roles_RoleId",
                schema: "users",
                table: "Permissions",
                column: "RoleId",
                principalSchema: "users",
                principalTable: "Roles",
                principalColumn: "ObjectId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Roles_RoleObjectId",
                schema: "users",
                table: "Permissions",
                column: "RoleObjectId",
                principalSchema: "users",
                principalTable: "Roles",
                principalColumn: "ObjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Roles_RoleId",
                schema: "users",
                table: "Permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Roles_RoleObjectId",
                schema: "users",
                table: "Permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Permissions",
                schema: "users",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_IsActive",
                schema: "users",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_RoleId",
                schema: "users",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_RoleObjectId",
                schema: "users",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "Action",
                schema: "users",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "Resource",
                schema: "users",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "RoleObjectId",
                schema: "users",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "Scope",
                schema: "users",
                table: "Permissions");

            migrationBuilder.RenameTable(
                name: "Permissions",
                schema: "users",
                newName: "RolePermissions",
                newSchema: "users");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "users",
                table: "RolePermissions",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "users",
                table: "RolePermissions",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<string>(
                name: "Permissions",
                schema: "users",
                table: "RolePermissions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermissions",
                schema: "users",
                table: "RolePermissions",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId_Permission",
                schema: "users",
                table: "RolePermissions",
                columns: new[] { "RoleId", "Permissions" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_Roles_RoleId",
                schema: "users",
                table: "RolePermissions",
                column: "RoleId",
                principalSchema: "users",
                principalTable: "Roles",
                principalColumn: "ObjectId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
