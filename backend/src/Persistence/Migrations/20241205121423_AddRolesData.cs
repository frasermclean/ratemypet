using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RateMyPet.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "EmailIndex",
                table: "Users");

            migrationBuilder.RenameIndex(
                name: "UserNameIndex",
                table: "Users",
                newName: "IX_Users_NormalizedUserName");

            migrationBuilder.RenameIndex(
                name: "RoleNameIndex",
                table: "Roles",
                newName: "IX_Roles_NormalizedName");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("57c523c9-0957-4834-8fce-ff37fa861c36"), null, "User", "USER" },
                    { new Guid("8e71eb35-2194-495b-b0e8-8690ebe7f918"), null, "Administrator", "ADMINISTRATOR" }
                });

            migrationBuilder.InsertData(
                table: "RoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "Permissions", "Posts.Add", new Guid("57c523c9-0957-4834-8fce-ff37fa861c36") },
                    { 2, "Permissions", "Posts.Edit.Owned", new Guid("57c523c9-0957-4834-8fce-ff37fa861c36") },
                    { 3, "Permissions", "Posts.Delete.Owned", new Guid("57c523c9-0957-4834-8fce-ff37fa861c36") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_NormalizedEmail",
                table: "Users",
                column: "NormalizedEmail",
                unique: true,
                filter: "[NormalizedEmail] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_NormalizedEmail",
                table: "Users");

            migrationBuilder.DeleteData(
                table: "RoleClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "RoleClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "RoleClaims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("8e71eb35-2194-495b-b0e8-8690ebe7f918"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("57c523c9-0957-4834-8fce-ff37fa861c36"));

            migrationBuilder.RenameIndex(
                name: "IX_Users_NormalizedUserName",
                table: "Users",
                newName: "UserNameIndex");

            migrationBuilder.RenameIndex(
                name: "IX_Roles_NormalizedName",
                table: "Roles",
                newName: "RoleNameIndex");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail");
        }
    }
}
