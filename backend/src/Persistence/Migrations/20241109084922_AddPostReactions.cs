using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RateMyPet.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPostReactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CrazyCount",
                table: "Posts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "FunnyCount",
                table: "Posts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "LikeCount",
                table: "Posts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "SadCount",
                table: "Posts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "WowCount",
                table: "Posts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CrazyCount",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "FunnyCount",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "LikeCount",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "SadCount",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "WowCount",
                table: "Posts");
        }
    }
}
