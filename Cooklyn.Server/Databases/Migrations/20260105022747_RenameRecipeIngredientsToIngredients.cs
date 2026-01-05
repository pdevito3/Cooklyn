using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cooklyn.Server.Databases.Migrations
{
    /// <inheritdoc />
    public partial class RenameRecipeIngredientsToIngredients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rename the table
            migrationBuilder.RenameTable(
                name: "recipe_ingredients",
                newName: "ingredients");

            // Rename the primary key constraint
            migrationBuilder.RenameIndex(
                name: "pk_recipe_ingredients",
                table: "ingredients",
                newName: "pk_ingredients");

            // Rename the foreign key constraint
            migrationBuilder.DropForeignKey(
                name: "fk_recipe_ingredients_recipes_recipe_id",
                table: "ingredients");

            migrationBuilder.AddForeignKey(
                name: "fk_ingredients_recipes_recipe_id",
                table: "ingredients",
                column: "recipe_id",
                principalTable: "recipes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            // Rename indexes
            migrationBuilder.RenameIndex(
                name: "ix_recipe_ingredients_recipe_id",
                table: "ingredients",
                newName: "ix_ingredients_recipe_id");

            migrationBuilder.RenameIndex(
                name: "ix_recipe_ingredients_sort_order",
                table: "ingredients",
                newName: "ix_ingredients_sort_order");

            // Make unit nullable (was previously required)
            migrationBuilder.AlterColumn<string>(
                name: "unit",
                table: "ingredients",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Make unit required again
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

            // Rename indexes back
            migrationBuilder.RenameIndex(
                name: "ix_ingredients_sort_order",
                table: "ingredients",
                newName: "ix_recipe_ingredients_sort_order");

            migrationBuilder.RenameIndex(
                name: "ix_ingredients_recipe_id",
                table: "ingredients",
                newName: "ix_recipe_ingredients_recipe_id");

            // Rename foreign key back
            migrationBuilder.DropForeignKey(
                name: "fk_ingredients_recipes_recipe_id",
                table: "ingredients");

            migrationBuilder.AddForeignKey(
                name: "fk_recipe_ingredients_recipes_recipe_id",
                table: "ingredients",
                column: "recipe_id",
                principalTable: "recipes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            // Rename primary key back
            migrationBuilder.RenameIndex(
                name: "pk_ingredients",
                table: "ingredients",
                newName: "pk_recipe_ingredients");

            // Rename table back
            migrationBuilder.RenameTable(
                name: "ingredients",
                newName: "recipe_ingredients");
        }
    }
}
