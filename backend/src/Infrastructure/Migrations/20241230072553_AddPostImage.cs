using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RateMyPet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPostImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsProcessed",
                table: "Posts");

            migrationBuilder.AddColumn<string>(
                name: "ImageFullBlobName",
                table: "Posts",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagePreviewBlobName",
                table: "Posts",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Status",
                table: "Posts",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Posts_Status",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ImageFullBlobName",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ImagePreviewBlobName",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Posts");

            migrationBuilder.AddColumn<bool>(
                name: "IsProcessed",
                table: "Posts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
