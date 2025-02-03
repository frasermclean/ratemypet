using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RateMyPet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPostTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAnalyzed",
                table: "Posts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_IsAnalyzed",
                table: "Posts",
                column: "IsAnalyzed");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Posts_IsAnalyzed",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "IsAnalyzed",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Posts");
        }
    }
}
