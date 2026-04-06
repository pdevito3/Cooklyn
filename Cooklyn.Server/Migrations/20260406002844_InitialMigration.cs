using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cooklyn.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cooklyn");

            migrationBuilder.CreateTable(
                name: "item_collections",
                schema: "cooklyn",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_item_collections", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "meal_plan_queues",
                schema: "cooklyn",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_meal_plan_queues", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "recent_searches",
                schema: "cooklyn",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    search_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    search_text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    resource_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    resource_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recent_searches", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "recipes",
                schema: "cooklyn",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    image_s3bucket = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    source = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    servings = table.Column<int>(type: "integer", nullable: true),
                    steps = table.Column<string>(type: "text", nullable: true),
                    notes = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    image_s3_key = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    rating = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recipes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "saved_filters",
                schema: "cooklyn",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    context = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    filter_state_json = table.Column<string>(type: "text", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_saved_filters", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "settings",
                schema: "cooklyn",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    key = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    value = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_settings", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "store_sections",
                schema: "cooklyn",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_store_sections", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "stores",
                schema: "cooklyn",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_stores", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tags",
                schema: "cooklyn",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tags", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ingredients",
                schema: "cooklyn",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    recipe_id = table.Column<string>(type: "text", nullable: false),
                    raw_text = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    amount = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: true),
                    amount_text = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    custom_unit = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    group_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ingredients", x => x.id);
                    table.ForeignKey(
                        name: "fk_ingredients_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalSchema: "cooklyn",
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "meal_plan_entries",
                schema: "cooklyn",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    recipe_id = table.Column<string>(type: "text", nullable: true),
                    title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    scale = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false, defaultValue: 1.0m),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    entry_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_meal_plan_entries", x => x.id);
                    table.ForeignKey(
                        name: "fk_meal_plan_entries_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalSchema: "cooklyn",
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "meal_plan_queue_items",
                schema: "cooklyn",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    queue_id = table.Column<string>(type: "text", nullable: false),
                    recipe_id = table.Column<string>(type: "text", nullable: true),
                    title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    scale = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false, defaultValue: 1.0m),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_meal_plan_queue_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_meal_plan_queue_items_meal_plan_queues_queue_id",
                        column: x => x.queue_id,
                        principalSchema: "cooklyn",
                        principalTable: "meal_plan_queues",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_meal_plan_queue_items_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalSchema: "cooklyn",
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "nutrition_infos",
                schema: "cooklyn",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    recipe_id = table.Column<string>(type: "text", nullable: false),
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
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_nutrition_infos", x => x.id);
                    table.ForeignKey(
                        name: "fk_nutrition_infos_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalSchema: "cooklyn",
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recipe_flag_entries",
                schema: "cooklyn",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    recipe_id = table.Column<string>(type: "text", nullable: false),
                    flag = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recipe_flag_entries", x => x.id);
                    table.ForeignKey(
                        name: "fk_recipe_flag_entries_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalSchema: "cooklyn",
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "item_category_mappings",
                schema: "cooklyn",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    normalized_name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    store_section_id = table.Column<string>(type: "text", nullable: false),
                    source = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_item_category_mappings", x => x.id);
                    table.ForeignKey(
                        name: "fk_item_category_mappings_store_sections_store_section_id",
                        column: x => x.store_section_id,
                        principalSchema: "cooklyn",
                        principalTable: "store_sections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "item_collection_items",
                schema: "cooklyn",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    item_collection_id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: true),
                    store_section_id = table.Column<string>(type: "text", nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_item_collection_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_item_collection_items_item_collections_item_collection_id",
                        column: x => x.item_collection_id,
                        principalSchema: "cooklyn",
                        principalTable: "item_collections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_item_collection_items_store_sections_store_section_id",
                        column: x => x.store_section_id,
                        principalSchema: "cooklyn",
                        principalTable: "store_sections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "shopping_lists",
                schema: "cooklyn",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    store_id = table.Column<string>(type: "text", nullable: true),
                    completed_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shopping_lists", x => x.id);
                    table.ForeignKey(
                        name: "fk_shopping_lists_stores_store_id",
                        column: x => x.store_id,
                        principalSchema: "cooklyn",
                        principalTable: "stores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "store_aisles",
                schema: "cooklyn",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    store_id = table.Column<string>(type: "text", nullable: false),
                    store_section_id = table.Column<string>(type: "text", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    custom_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_store_aisles", x => x.id);
                    table.ForeignKey(
                        name: "fk_store_aisles_store_sections_store_section_id",
                        column: x => x.store_section_id,
                        principalSchema: "cooklyn",
                        principalTable: "store_sections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_store_aisles_stores_store_id",
                        column: x => x.store_id,
                        principalSchema: "cooklyn",
                        principalTable: "stores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "store_default_collections",
                schema: "cooklyn",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    store_id = table.Column<string>(type: "text", nullable: false),
                    item_collection_id = table.Column<string>(type: "text", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_store_default_collections", x => x.id);
                    table.ForeignKey(
                        name: "fk_store_default_collections_item_collections_item_collection_",
                        column: x => x.item_collection_id,
                        principalSchema: "cooklyn",
                        principalTable: "item_collections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_store_default_collections_stores_store_id",
                        column: x => x.store_id,
                        principalSchema: "cooklyn",
                        principalTable: "stores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recipe_tags",
                schema: "cooklyn",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    recipe_id = table.Column<string>(type: "text", nullable: false),
                    tag_id = table.Column<string>(type: "text", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recipe_tags", x => x.id);
                    table.ForeignKey(
                        name: "fk_recipe_tags_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalSchema: "cooklyn",
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_recipe_tags_tags_tag_id",
                        column: x => x.tag_id,
                        principalSchema: "cooklyn",
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "shopping_list_items",
                schema: "cooklyn",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    shopping_list_id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: true),
                    store_section_id = table.Column<string>(type: "text", nullable: true),
                    is_checked = table.Column<bool>(type: "boolean", nullable: false),
                    checked_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shopping_list_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_shopping_list_items_shopping_lists_shopping_list_id",
                        column: x => x.shopping_list_id,
                        principalSchema: "cooklyn",
                        principalTable: "shopping_lists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_shopping_list_items_store_sections_store_section_id",
                        column: x => x.store_section_id,
                        principalSchema: "cooklyn",
                        principalTable: "store_sections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "shopping_list_item_recipe_sources",
                schema: "cooklyn",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    shopping_list_item_id = table.Column<string>(type: "text", nullable: false),
                    recipe_id = table.Column<string>(type: "text", nullable: false),
                    original_quantity = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: true),
                    original_unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shopping_list_item_recipe_sources", x => x.id);
                    table.ForeignKey(
                        name: "fk_shopping_list_item_recipe_sources_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalSchema: "cooklyn",
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_shopping_list_item_recipe_sources_shopping_list_items_shopp",
                        column: x => x.shopping_list_item_id,
                        principalSchema: "cooklyn",
                        principalTable: "shopping_list_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ingredients_recipe_id",
                schema: "cooklyn",
                table: "ingredients",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "ix_item_category_mappings_normalized_name",
                schema: "cooklyn",
                table: "item_category_mappings",
                column: "normalized_name",
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_item_category_mappings_store_section_id",
                schema: "cooklyn",
                table: "item_category_mappings",
                column: "store_section_id");

            migrationBuilder.CreateIndex(
                name: "ix_item_collection_items_item_collection_id",
                schema: "cooklyn",
                table: "item_collection_items",
                column: "item_collection_id");

            migrationBuilder.CreateIndex(
                name: "ix_item_collection_items_store_section_id",
                schema: "cooklyn",
                table: "item_collection_items",
                column: "store_section_id");

            migrationBuilder.CreateIndex(
                name: "ix_item_collections_name",
                schema: "cooklyn",
                table: "item_collections",
                column: "name",
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_meal_plan_entries_date",
                schema: "cooklyn",
                table: "meal_plan_entries",
                column: "date");

            migrationBuilder.CreateIndex(
                name: "ix_meal_plan_entries_recipe_id",
                schema: "cooklyn",
                table: "meal_plan_entries",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "ix_meal_plan_queue_items_queue_id",
                schema: "cooklyn",
                table: "meal_plan_queue_items",
                column: "queue_id");

            migrationBuilder.CreateIndex(
                name: "ix_meal_plan_queue_items_recipe_id",
                schema: "cooklyn",
                table: "meal_plan_queue_items",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "ix_nutrition_infos_recipe_id",
                schema: "cooklyn",
                table: "nutrition_infos",
                column: "recipe_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_recent_searches_created_on",
                schema: "cooklyn",
                table: "recent_searches",
                column: "created_on",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "ix_recent_searches_search_type_search_text_resource_type_resou",
                schema: "cooklyn",
                table: "recent_searches",
                columns: new[] { "search_type", "search_text", "resource_type", "resource_id" });

            migrationBuilder.CreateIndex(
                name: "ix_recipe_flag_entries_recipe_id",
                schema: "cooklyn",
                table: "recipe_flag_entries",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "ix_recipe_tags_recipe_id",
                schema: "cooklyn",
                table: "recipe_tags",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "ix_recipe_tags_recipe_id_tag_id",
                schema: "cooklyn",
                table: "recipe_tags",
                columns: new[] { "recipe_id", "tag_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_recipe_tags_tag_id",
                schema: "cooklyn",
                table: "recipe_tags",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "ix_recipes_title",
                schema: "cooklyn",
                table: "recipes",
                column: "title");

            migrationBuilder.CreateIndex(
                name: "ix_saved_filters_context",
                schema: "cooklyn",
                table: "saved_filters",
                column: "context");

            migrationBuilder.CreateIndex(
                name: "ix_saved_filters_name_context",
                schema: "cooklyn",
                table: "saved_filters",
                columns: new[] { "name", "context" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_settings_key",
                schema: "cooklyn",
                table: "settings",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_shopping_list_item_recipe_sources_recipe_id",
                schema: "cooklyn",
                table: "shopping_list_item_recipe_sources",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "ix_shopping_list_item_recipe_sources_shopping_list_item_id",
                schema: "cooklyn",
                table: "shopping_list_item_recipe_sources",
                column: "shopping_list_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_shopping_list_item_recipe_sources_shopping_list_item_id_rec",
                schema: "cooklyn",
                table: "shopping_list_item_recipe_sources",
                columns: new[] { "shopping_list_item_id", "recipe_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_shopping_list_items_shopping_list_id",
                schema: "cooklyn",
                table: "shopping_list_items",
                column: "shopping_list_id");

            migrationBuilder.CreateIndex(
                name: "ix_shopping_list_items_store_section_id",
                schema: "cooklyn",
                table: "shopping_list_items",
                column: "store_section_id");

            migrationBuilder.CreateIndex(
                name: "ix_shopping_lists_store_id",
                schema: "cooklyn",
                table: "shopping_lists",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "ix_store_aisles_store_id",
                schema: "cooklyn",
                table: "store_aisles",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "ix_store_aisles_store_id_store_section_id",
                schema: "cooklyn",
                table: "store_aisles",
                columns: new[] { "store_id", "store_section_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_store_aisles_store_section_id",
                schema: "cooklyn",
                table: "store_aisles",
                column: "store_section_id");

            migrationBuilder.CreateIndex(
                name: "ix_store_default_collections_item_collection_id",
                schema: "cooklyn",
                table: "store_default_collections",
                column: "item_collection_id");

            migrationBuilder.CreateIndex(
                name: "ix_store_default_collections_store_id",
                schema: "cooklyn",
                table: "store_default_collections",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "ix_store_default_collections_store_id_item_collection_id",
                schema: "cooklyn",
                table: "store_default_collections",
                columns: new[] { "store_id", "item_collection_id" },
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_store_sections_name",
                schema: "cooklyn",
                table: "store_sections",
                column: "name",
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_stores_name",
                schema: "cooklyn",
                table: "stores",
                column: "name",
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_tags_name",
                schema: "cooklyn",
                table: "tags",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ingredients",
                schema: "cooklyn");

            migrationBuilder.DropTable(
                name: "item_category_mappings",
                schema: "cooklyn");

            migrationBuilder.DropTable(
                name: "item_collection_items",
                schema: "cooklyn");

            migrationBuilder.DropTable(
                name: "meal_plan_entries",
                schema: "cooklyn");

            migrationBuilder.DropTable(
                name: "meal_plan_queue_items",
                schema: "cooklyn");

            migrationBuilder.DropTable(
                name: "nutrition_infos",
                schema: "cooklyn");

            migrationBuilder.DropTable(
                name: "recent_searches",
                schema: "cooklyn");

            migrationBuilder.DropTable(
                name: "recipe_flag_entries",
                schema: "cooklyn");

            migrationBuilder.DropTable(
                name: "recipe_tags",
                schema: "cooklyn");

            migrationBuilder.DropTable(
                name: "saved_filters",
                schema: "cooklyn");

            migrationBuilder.DropTable(
                name: "settings",
                schema: "cooklyn");

            migrationBuilder.DropTable(
                name: "shopping_list_item_recipe_sources",
                schema: "cooklyn");

            migrationBuilder.DropTable(
                name: "store_aisles",
                schema: "cooklyn");

            migrationBuilder.DropTable(
                name: "store_default_collections",
                schema: "cooklyn");

            migrationBuilder.DropTable(
                name: "meal_plan_queues",
                schema: "cooklyn");

            migrationBuilder.DropTable(
                name: "tags",
                schema: "cooklyn");

            migrationBuilder.DropTable(
                name: "recipes",
                schema: "cooklyn");

            migrationBuilder.DropTable(
                name: "shopping_list_items",
                schema: "cooklyn");

            migrationBuilder.DropTable(
                name: "item_collections",
                schema: "cooklyn");

            migrationBuilder.DropTable(
                name: "shopping_lists",
                schema: "cooklyn");

            migrationBuilder.DropTable(
                name: "store_sections",
                schema: "cooklyn");

            migrationBuilder.DropTable(
                name: "stores",
                schema: "cooklyn");
        }
    }
}
