using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cooklyn.Server.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsFavorite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Preserve favorites: promote IsFavorite=true recipes that are unrated to "Loved It"
            migrationBuilder.Sql(
                """UPDATE "recipes" SET rating = 'Loved It' WHERE is_favorite = true AND rating = 'Not Rated'""");

            migrationBuilder.DropIndex(
                name: "ix_recipes_is_favorite",
                table: "recipes");

            migrationBuilder.DropColumn(
                name: "is_favorite",
                table: "recipes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_favorite",
                table: "recipes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "ix_recipes_is_favorite",
                table: "recipes",
                column: "is_favorite");
        }
    }
}
