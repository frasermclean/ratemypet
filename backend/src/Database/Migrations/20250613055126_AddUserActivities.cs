using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RateMyPet.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddUserActivities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Posts_Slug",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "LastSeenUtc",
                table: "Users",
                newName: "LastActivityUtc");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Posts",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                defaultValueSql: "CONCAT('post-', LOWER(CONVERT(varchar(36), NEWID())))",
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "Posts",
                type: "datetime2(2)",
                precision: 2,
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Posts_Slug",
                table: "Posts",
                column: "Slug");

            migrationBuilder.CreateTable(
                name: "UserActivities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "char(4)", nullable: false),
                    TimestampUtc = table.Column<DateTime>(type: "datetime2(2)", precision: 2, nullable: false, defaultValueSql: "getutcdate()"),
                    Discriminator = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserActivities_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserActivities_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserActivities_PostId",
                table: "UserActivities",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivities_UserId",
                table: "UserActivities",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserActivities");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Posts_Slug",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "LastActivityUtc",
                table: "Users",
                newName: "LastSeenUtc");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Posts",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60,
                oldDefaultValueSql: "CONCAT('post-', LOWER(CONVERT(varchar(36), NEWID())))");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Slug",
                table: "Posts",
                column: "Slug");
        }
    }
}
