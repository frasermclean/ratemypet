using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RateMyPet.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { new Guid("fb8ad061-3a62-45f9-2202-08dd15f6fc85"), 0, "initial", "dev@frasermclean.com", true, false, null, "DEV@FRASERMCLEAN.COM", "FRASERMCLEAN", null, null, false, "initial", false, "frasermclean" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("57c523c9-0957-4834-8fce-ff37fa861c36"), new Guid("fb8ad061-3a62-45f9-2202-08dd15f6fc85") },
                    { new Guid("8e71eb35-2194-495b-b0e8-8690ebe7f918"), new Guid("fb8ad061-3a62-45f9-2202-08dd15f6fc85") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("57c523c9-0957-4834-8fce-ff37fa861c36"), new Guid("fb8ad061-3a62-45f9-2202-08dd15f6fc85") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("8e71eb35-2194-495b-b0e8-8690ebe7f918"), new Guid("fb8ad061-3a62-45f9-2202-08dd15f6fc85") });

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("fb8ad061-3a62-45f9-2202-08dd15f6fc85"));
        }
    }
}
