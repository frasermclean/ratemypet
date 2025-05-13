using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RateMyPet.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPostImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsProcessed",
                table: "Posts",
                newName: "ImageIsProcessed");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Posts",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "newid()");

            migrationBuilder.AddColumn<string>(
                name: "ImageFileName",
                table: "Posts",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ImageHeight",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ImageMimeType",
                table: "Posts",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "ImageSize",
                table: "Posts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "ImageWidth",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageFileName",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ImageHeight",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ImageMimeType",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ImageSize",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ImageWidth",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "ImageIsProcessed",
                table: "Posts",
                newName: "IsProcessed");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Posts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "newid()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }
    }
}
