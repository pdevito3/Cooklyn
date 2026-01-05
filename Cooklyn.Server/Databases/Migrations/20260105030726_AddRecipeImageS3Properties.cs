using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cooklyn.Server.Databases.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipeImageS3Properties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "image_url",
                table: "recipes");

            migrationBuilder.AddColumn<string>(
                name: "image_s3_key",
                table: "recipes",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "image_s3bucket",
                table: "recipes",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "image_s3_key",
                table: "recipes");

            migrationBuilder.DropColumn(
                name: "image_s3bucket",
                table: "recipes");

            migrationBuilder.AddColumn<string>(
                name: "image_url",
                table: "recipes",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);
        }
    }
}
