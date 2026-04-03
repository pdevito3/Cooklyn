using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cooklyn.Server.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_item_category_mappings_tenants_tenant_id",
                table: "item_category_mappings");

            migrationBuilder.DropForeignKey(
                name: "fk_item_collections_tenants_tenant_id",
                table: "item_collections");

            migrationBuilder.DropForeignKey(
                name: "fk_meal_plan_entries_tenants_tenant_id",
                table: "meal_plan_entries");

            migrationBuilder.DropForeignKey(
                name: "fk_meal_plan_queues_tenants_tenant_id",
                table: "meal_plan_queues");

            migrationBuilder.DropForeignKey(
                name: "fk_recent_searches_tenants_tenant_id",
                table: "recent_searches");

            migrationBuilder.DropForeignKey(
                name: "fk_recipes_tenants_tenant_id",
                table: "recipes");

            migrationBuilder.DropForeignKey(
                name: "fk_saved_filters_tenants_tenant_id",
                table: "saved_filters");

            migrationBuilder.DropForeignKey(
                name: "fk_shopping_lists_tenants_tenant_id",
                table: "shopping_lists");

            migrationBuilder.DropForeignKey(
                name: "fk_store_sections_tenants_tenant_id",
                table: "store_sections");

            migrationBuilder.DropForeignKey(
                name: "fk_stores_tenants_tenant_id",
                table: "stores");

            migrationBuilder.DropForeignKey(
                name: "fk_tags_tenants_tenant_id",
                table: "tags");

            migrationBuilder.DropTable(
                name: "user_permissions");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "tenants");

            migrationBuilder.DropIndex(
                name: "ix_tags_tenant_id",
                table: "tags");

            migrationBuilder.DropIndex(
                name: "ix_tags_tenant_id_name",
                table: "tags");

            migrationBuilder.DropIndex(
                name: "ix_stores_tenant_id",
                table: "stores");

            migrationBuilder.DropIndex(
                name: "ix_stores_tenant_id_name",
                table: "stores");

            migrationBuilder.DropIndex(
                name: "ix_store_sections_tenant_id",
                table: "store_sections");

            migrationBuilder.DropIndex(
                name: "ix_store_sections_tenant_id_name",
                table: "store_sections");

            migrationBuilder.DropIndex(
                name: "ix_shopping_lists_tenant_id",
                table: "shopping_lists");

            migrationBuilder.DropIndex(
                name: "ix_saved_filters_tenant_id_context",
                table: "saved_filters");

            migrationBuilder.DropIndex(
                name: "ix_saved_filters_tenant_id_name_context",
                table: "saved_filters");

            migrationBuilder.DropIndex(
                name: "ix_recipes_tenant_id",
                table: "recipes");

            migrationBuilder.DropIndex(
                name: "ix_recent_searches_tenant_id_created_on",
                table: "recent_searches");

            migrationBuilder.DropIndex(
                name: "ix_recent_searches_tenant_id_search_type_search_text_resource_",
                table: "recent_searches");

            migrationBuilder.DropIndex(
                name: "ix_meal_plan_queues_tenant_id",
                table: "meal_plan_queues");

            migrationBuilder.DropIndex(
                name: "ix_meal_plan_entries_tenant_id_date",
                table: "meal_plan_entries");

            migrationBuilder.DropIndex(
                name: "ix_item_collections_tenant_id",
                table: "item_collections");

            migrationBuilder.DropIndex(
                name: "ix_item_collections_tenant_id_name",
                table: "item_collections");

            migrationBuilder.DropIndex(
                name: "ix_item_category_mappings_tenant_id",
                table: "item_category_mappings");

            migrationBuilder.DropIndex(
                name: "ix_item_category_mappings_tenant_id_normalized_name",
                table: "item_category_mappings");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "last_modified_by",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "stores");

            migrationBuilder.DropColumn(
                name: "last_modified_by",
                table: "stores");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                table: "stores");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "store_sections");

            migrationBuilder.DropColumn(
                name: "last_modified_by",
                table: "store_sections");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                table: "store_sections");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "store_default_collections");

            migrationBuilder.DropColumn(
                name: "last_modified_by",
                table: "store_default_collections");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "store_aisles");

            migrationBuilder.DropColumn(
                name: "last_modified_by",
                table: "store_aisles");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "shopping_lists");

            migrationBuilder.DropColumn(
                name: "last_modified_by",
                table: "shopping_lists");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                table: "shopping_lists");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "shopping_list_items");

            migrationBuilder.DropColumn(
                name: "last_modified_by",
                table: "shopping_list_items");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "shopping_list_item_recipe_sources");

            migrationBuilder.DropColumn(
                name: "last_modified_by",
                table: "shopping_list_item_recipe_sources");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "saved_filters");

            migrationBuilder.DropColumn(
                name: "last_modified_by",
                table: "saved_filters");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                table: "saved_filters");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "recipes");

            migrationBuilder.DropColumn(
                name: "last_modified_by",
                table: "recipes");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                table: "recipes");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "recipe_tags");

            migrationBuilder.DropColumn(
                name: "last_modified_by",
                table: "recipe_tags");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "recipe_flag_entries");

            migrationBuilder.DropColumn(
                name: "last_modified_by",
                table: "recipe_flag_entries");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "recent_searches");

            migrationBuilder.DropColumn(
                name: "last_modified_by",
                table: "recent_searches");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                table: "recent_searches");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "nutrition_infos");

            migrationBuilder.DropColumn(
                name: "last_modified_by",
                table: "nutrition_infos");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "meal_plan_queues");

            migrationBuilder.DropColumn(
                name: "last_modified_by",
                table: "meal_plan_queues");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                table: "meal_plan_queues");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "meal_plan_queue_items");

            migrationBuilder.DropColumn(
                name: "last_modified_by",
                table: "meal_plan_queue_items");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "meal_plan_entries");

            migrationBuilder.DropColumn(
                name: "last_modified_by",
                table: "meal_plan_entries");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                table: "meal_plan_entries");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "item_collections");

            migrationBuilder.DropColumn(
                name: "last_modified_by",
                table: "item_collections");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                table: "item_collections");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "item_collection_items");

            migrationBuilder.DropColumn(
                name: "last_modified_by",
                table: "item_collection_items");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "item_category_mappings");

            migrationBuilder.DropColumn(
                name: "last_modified_by",
                table: "item_category_mappings");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                table: "item_category_mappings");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "ingredients");

            migrationBuilder.DropColumn(
                name: "last_modified_by",
                table: "ingredients");

            migrationBuilder.CreateTable(
                name: "settings",
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

            migrationBuilder.CreateIndex(
                name: "ix_tags_name",
                table: "tags",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_stores_name",
                table: "stores",
                column: "name",
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_store_sections_name",
                table: "store_sections",
                column: "name",
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_saved_filters_context",
                table: "saved_filters",
                column: "context");

            migrationBuilder.CreateIndex(
                name: "ix_saved_filters_name_context",
                table: "saved_filters",
                columns: new[] { "name", "context" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_recent_searches_created_on",
                table: "recent_searches",
                column: "created_on",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "ix_recent_searches_search_type_search_text_resource_type_resou",
                table: "recent_searches",
                columns: new[] { "search_type", "search_text", "resource_type", "resource_id" });

            migrationBuilder.CreateIndex(
                name: "ix_meal_plan_entries_date",
                table: "meal_plan_entries",
                column: "date");

            migrationBuilder.CreateIndex(
                name: "ix_item_collections_name",
                table: "item_collections",
                column: "name",
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_item_category_mappings_normalized_name",
                table: "item_category_mappings",
                column: "normalized_name",
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_settings_key",
                table: "settings",
                column: "key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "settings");

            migrationBuilder.DropIndex(
                name: "ix_tags_name",
                table: "tags");

            migrationBuilder.DropIndex(
                name: "ix_stores_name",
                table: "stores");

            migrationBuilder.DropIndex(
                name: "ix_store_sections_name",
                table: "store_sections");

            migrationBuilder.DropIndex(
                name: "ix_saved_filters_context",
                table: "saved_filters");

            migrationBuilder.DropIndex(
                name: "ix_saved_filters_name_context",
                table: "saved_filters");

            migrationBuilder.DropIndex(
                name: "ix_recent_searches_created_on",
                table: "recent_searches");

            migrationBuilder.DropIndex(
                name: "ix_recent_searches_search_type_search_text_resource_type_resou",
                table: "recent_searches");

            migrationBuilder.DropIndex(
                name: "ix_meal_plan_entries_date",
                table: "meal_plan_entries");

            migrationBuilder.DropIndex(
                name: "ix_item_collections_name",
                table: "item_collections");

            migrationBuilder.DropIndex(
                name: "ix_item_category_mappings_normalized_name",
                table: "item_category_mappings");

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "tags",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_modified_by",
                table: "tags",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tenant_id",
                table: "tags",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "stores",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_modified_by",
                table: "stores",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tenant_id",
                table: "stores",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "store_sections",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_modified_by",
                table: "store_sections",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tenant_id",
                table: "store_sections",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "store_default_collections",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_modified_by",
                table: "store_default_collections",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "store_aisles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_modified_by",
                table: "store_aisles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "shopping_lists",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_modified_by",
                table: "shopping_lists",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tenant_id",
                table: "shopping_lists",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "shopping_list_items",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_modified_by",
                table: "shopping_list_items",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "shopping_list_item_recipe_sources",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_modified_by",
                table: "shopping_list_item_recipe_sources",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "saved_filters",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_modified_by",
                table: "saved_filters",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tenant_id",
                table: "saved_filters",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "recipes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_modified_by",
                table: "recipes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tenant_id",
                table: "recipes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "recipe_tags",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_modified_by",
                table: "recipe_tags",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "recipe_flag_entries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_modified_by",
                table: "recipe_flag_entries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "recent_searches",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_modified_by",
                table: "recent_searches",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tenant_id",
                table: "recent_searches",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "nutrition_infos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_modified_by",
                table: "nutrition_infos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "meal_plan_queues",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_modified_by",
                table: "meal_plan_queues",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tenant_id",
                table: "meal_plan_queues",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "meal_plan_queue_items",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_modified_by",
                table: "meal_plan_queue_items",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "meal_plan_entries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_modified_by",
                table: "meal_plan_entries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tenant_id",
                table: "meal_plan_entries",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "item_collections",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_modified_by",
                table: "item_collections",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tenant_id",
                table: "item_collections",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "item_collection_items",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_modified_by",
                table: "item_collection_items",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "item_category_mappings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_modified_by",
                table: "item_category_mappings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tenant_id",
                table: "item_category_mappings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "ingredients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_modified_by",
                table: "ingredients",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "tenants",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenants", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    default_store_id = table.Column<string>(type: "text", nullable: true),
                    first_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    identifier = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    tenant_id = table.Column<string>(type: "text", nullable: false),
                    username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_stores_default_store_id",
                        column: x => x.default_store_id,
                        principalTable: "stores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_users_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_permissions",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    permission = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_permissions", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_permissions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_tags_tenant_id",
                table: "tags",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_tags_tenant_id_name",
                table: "tags",
                columns: new[] { "tenant_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_stores_tenant_id",
                table: "stores",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_stores_tenant_id_name",
                table: "stores",
                columns: new[] { "tenant_id", "name" },
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_store_sections_tenant_id",
                table: "store_sections",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_store_sections_tenant_id_name",
                table: "store_sections",
                columns: new[] { "tenant_id", "name" },
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_shopping_lists_tenant_id",
                table: "shopping_lists",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_saved_filters_tenant_id_context",
                table: "saved_filters",
                columns: new[] { "tenant_id", "context" });

            migrationBuilder.CreateIndex(
                name: "ix_saved_filters_tenant_id_name_context",
                table: "saved_filters",
                columns: new[] { "tenant_id", "name", "context" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_recipes_tenant_id",
                table: "recipes",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_recent_searches_tenant_id_created_on",
                table: "recent_searches",
                columns: new[] { "tenant_id", "created_on" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "ix_recent_searches_tenant_id_search_type_search_text_resource_",
                table: "recent_searches",
                columns: new[] { "tenant_id", "search_type", "search_text", "resource_type", "resource_id" });

            migrationBuilder.CreateIndex(
                name: "ix_meal_plan_queues_tenant_id",
                table: "meal_plan_queues",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_meal_plan_entries_tenant_id_date",
                table: "meal_plan_entries",
                columns: new[] { "tenant_id", "date" });

            migrationBuilder.CreateIndex(
                name: "ix_item_collections_tenant_id",
                table: "item_collections",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_item_collections_tenant_id_name",
                table: "item_collections",
                columns: new[] { "tenant_id", "name" },
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_item_category_mappings_tenant_id",
                table: "item_category_mappings",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_item_category_mappings_tenant_id_normalized_name",
                table: "item_category_mappings",
                columns: new[] { "tenant_id", "normalized_name" },
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_tenants_name",
                table: "tenants",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_permissions_user_id",
                table: "user_permissions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_default_store_id",
                table: "users",
                column: "default_store_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_identifier",
                table: "users",
                column: "identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_tenant_id",
                table: "users",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_username",
                table: "users",
                column: "username");

            migrationBuilder.AddForeignKey(
                name: "fk_item_category_mappings_tenants_tenant_id",
                table: "item_category_mappings",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_item_collections_tenants_tenant_id",
                table: "item_collections",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_meal_plan_entries_tenants_tenant_id",
                table: "meal_plan_entries",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_meal_plan_queues_tenants_tenant_id",
                table: "meal_plan_queues",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_recent_searches_tenants_tenant_id",
                table: "recent_searches",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_recipes_tenants_tenant_id",
                table: "recipes",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_saved_filters_tenants_tenant_id",
                table: "saved_filters",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_shopping_lists_tenants_tenant_id",
                table: "shopping_lists",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_store_sections_tenants_tenant_id",
                table: "store_sections",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_stores_tenants_tenant_id",
                table: "stores",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_tags_tenants_tenant_id",
                table: "tags",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
