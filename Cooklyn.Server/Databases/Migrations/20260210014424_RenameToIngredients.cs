using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cooklyn.Server.Databases.Migrations
{
    /// <inheritdoc />
    public partial class RenameToIngredients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rename table
            migrationBuilder.RenameTable(
                name: "recipe_ingredients",
                newName: "ingredients");

            // Rename constraints and indexes
            migrationBuilder.Sql(
                "ALTER TABLE ingredients RENAME CONSTRAINT pk_recipe_ingredients TO pk_ingredients;");
            migrationBuilder.Sql(
                "ALTER TABLE ingredients RENAME CONSTRAINT fk_recipe_ingredients_recipes_recipe_id TO fk_ingredients_recipes_recipe_id;");
            migrationBuilder.RenameIndex(
                table: "ingredients",
                name: "ix_recipe_ingredients_recipe_id",
                newName: "ix_ingredients_recipe_id");

            // Convert unit column from nullable to non-nullable (IngredientUnit value object)
            migrationBuilder.Sql(
                "UPDATE ingredients SET unit = '' WHERE unit IS NULL;");
            migrationBuilder.AlterColumn<string>(
                name: "unit",
                table: "ingredients",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert unit column to nullable
            migrationBuilder.AlterColumn<string>(
                name: "unit",
                table: "ingredients",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            // Rename constraints and indexes back
            migrationBuilder.RenameIndex(
                table: "ingredients",
                name: "ix_ingredients_recipe_id",
                newName: "ix_recipe_ingredients_recipe_id");
            migrationBuilder.Sql(
                "ALTER TABLE ingredients RENAME CONSTRAINT fk_ingredients_recipes_recipe_id TO fk_recipe_ingredients_recipes_recipe_id;");
            migrationBuilder.Sql(
                "ALTER TABLE ingredients RENAME CONSTRAINT pk_ingredients TO pk_recipe_ingredients;");

            // Rename table back
            migrationBuilder.RenameTable(
                name: "ingredients",
                newName: "recipe_ingredients");
        }
    }
}
