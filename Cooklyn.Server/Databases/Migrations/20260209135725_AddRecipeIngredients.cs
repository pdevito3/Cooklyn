using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cooklyn.Server.Databases.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipeIngredients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "recipe_ingredients",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                    raw_text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    amount = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: true),
                    amount_text = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    custom_unit = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    group_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recipe_ingredients", x => x.id);
                    table.ForeignKey(
                        name: "fk_recipe_ingredients_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_recipe_ingredients_recipe_id",
                table: "recipe_ingredients",
                column: "recipe_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "recipe_ingredients");
        }
    }
}
