using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cooklyn.Server.Databases.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipesAndTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "recipes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    image_url = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    source = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    is_favorite = table.Column<bool>(type: "boolean", nullable: false),
                    servings = table.Column<int>(type: "integer", nullable: true),
                    steps = table.Column<string>(type: "text", nullable: true),
                    notes = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    rating = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recipes", x => x.id);
                    table.ForeignKey(
                        name: "fk_recipes_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tags", x => x.id);
                    table.ForeignKey(
                        name: "fk_tags_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "nutrition_infos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                    calories = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    total_fat_grams = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    saturated_fat_grams = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    trans_fat_grams = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    cholesterol_milligrams = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    sodium_milligrams = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    total_carbohydrates_grams = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    dietary_fiber_grams = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    total_sugars_grams = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    added_sugars_grams = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    protein_grams = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    vitamin_d_percent = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    calcium_percent = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    iron_percent = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    potassium_percent = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    is_manually_entered = table.Column<bool>(type: "boolean", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_nutrition_infos", x => x.id);
                    table.ForeignKey(
                        name: "fk_nutrition_infos_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recipe_flag_entries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                    flag = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recipe_flag_entries", x => x.id);
                    table.ForeignKey(
                        name: "fk_recipe_flag_entries_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recipe_ingredients",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    custom_unit = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
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

            migrationBuilder.CreateTable(
                name: "recipe_tags",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tag_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recipe_tags", x => x.id);
                    table.ForeignKey(
                        name: "fk_recipe_tags_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_recipe_tags_tags_tag_id",
                        column: x => x.tag_id,
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_nutrition_infos_recipe_id",
                table: "nutrition_infos",
                column: "recipe_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_recipe_flag_entries_recipe_id",
                table: "recipe_flag_entries",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "ix_recipe_ingredients_recipe_id",
                table: "recipe_ingredients",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "ix_recipe_ingredients_sort_order",
                table: "recipe_ingredients",
                column: "sort_order");

            migrationBuilder.CreateIndex(
                name: "ix_recipe_tags_recipe_id",
                table: "recipe_tags",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "ix_recipe_tags_recipe_id_tag_id",
                table: "recipe_tags",
                columns: new[] { "recipe_id", "tag_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_recipe_tags_tag_id",
                table: "recipe_tags",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "ix_recipes_is_favorite",
                table: "recipes",
                column: "is_favorite");

            migrationBuilder.CreateIndex(
                name: "ix_recipes_tenant_id",
                table: "recipes",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_recipes_title",
                table: "recipes",
                column: "title");

            migrationBuilder.CreateIndex(
                name: "ix_tags_tenant_id",
                table: "tags",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_tags_tenant_id_name",
                table: "tags",
                columns: new[] { "tenant_id", "name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "nutrition_infos");

            migrationBuilder.DropTable(
                name: "recipe_flag_entries");

            migrationBuilder.DropTable(
                name: "recipe_ingredients");

            migrationBuilder.DropTable(
                name: "recipe_tags");

            migrationBuilder.DropTable(
                name: "recipes");

            migrationBuilder.DropTable(
                name: "tags");
        }
    }
}
