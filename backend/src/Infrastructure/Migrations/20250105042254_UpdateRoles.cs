using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RateMyPet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("57c523c9-0957-4834-8fce-ff37fa861c36"),
                columns: new[] { "Name", "NormalizedName" },
                values: new object[] { "Contributor", "CONTRIBUTOR" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "RoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "Permissions", "Posts.Add", new Guid("57c523c9-0957-4834-8fce-ff37fa861c36") },
                    { 2, "Permissions", "Posts.Edit.Owned", new Guid("57c523c9-0957-4834-8fce-ff37fa861c36") },
                    { 3, "Permissions", "Posts.Delete.Owned", new Guid("57c523c9-0957-4834-8fce-ff37fa861c36") }
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("57c523c9-0957-4834-8fce-ff37fa861c36"),
                columns: new[] { "Name", "NormalizedName" },
                values: new object[] { "User", "USER" });
        }
    }
}
