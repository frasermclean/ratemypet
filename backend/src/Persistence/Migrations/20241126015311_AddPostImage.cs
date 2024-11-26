using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RateMyPet.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPostImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageBlobName",
                table: "Posts",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ImageHeight",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
                name: "ImageBlobName",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ImageHeight",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ImageWidth",
                table: "Posts");
        }
    }
}
