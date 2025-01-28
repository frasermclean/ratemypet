using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RateMyPet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPostSlug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Posts",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                defaultValueSql: "newid()");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Posts_Slug",
                table: "Posts",
                column: "Slug");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Posts_Slug",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Posts");
        }
    }
}
