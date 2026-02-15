using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cooklyn.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddStoresShoppingListsAndCollections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "default_store_id",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "item_collections",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    tenant_id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_item_collections", x => x.id);
                    table.ForeignKey(
                        name: "fk_item_collections_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "store_sections",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    tenant_id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_store_sections", x => x.id);
                    table.ForeignKey(
                        name: "fk_store_sections_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "stores",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    tenant_id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_stores", x => x.id);
                    table.ForeignKey(
                        name: "fk_stores_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "item_collection_items",
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
                    created_by = table.Column<string>(type: "text", nullable: true),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_item_collection_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_item_collection_items_item_collections_item_collection_id",
                        column: x => x.item_collection_id,
                        principalTable: "item_collections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_item_collection_items_store_sections_store_section_id",
                        column: x => x.store_section_id,
                        principalTable: "store_sections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "shopping_lists",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    tenant_id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    store_id = table.Column<string>(type: "text", nullable: true),
                    completed_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shopping_lists", x => x.id);
                    table.ForeignKey(
                        name: "fk_shopping_lists_stores_store_id",
                        column: x => x.store_id,
                        principalTable: "stores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_shopping_lists_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "store_aisles",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    store_id = table.Column<string>(type: "text", nullable: false),
                    store_section_id = table.Column<string>(type: "text", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    custom_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_store_aisles", x => x.id);
                    table.ForeignKey(
                        name: "fk_store_aisles_store_sections_store_section_id",
                        column: x => x.store_section_id,
                        principalTable: "store_sections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_store_aisles_stores_store_id",
                        column: x => x.store_id,
                        principalTable: "stores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "store_default_collections",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    store_id = table.Column<string>(type: "text", nullable: false),
                    item_collection_id = table.Column<string>(type: "text", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_store_default_collections", x => x.id);
                    table.ForeignKey(
                        name: "fk_store_default_collections_item_collections_item_collection_",
                        column: x => x.item_collection_id,
                        principalTable: "item_collections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_store_default_collections_stores_store_id",
                        column: x => x.store_id,
                        principalTable: "stores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "shopping_list_items",
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
                    created_by = table.Column<string>(type: "text", nullable: true),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shopping_list_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_shopping_list_items_shopping_lists_shopping_list_id",
                        column: x => x.shopping_list_id,
                        principalTable: "shopping_lists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_shopping_list_items_store_sections_store_section_id",
                        column: x => x.store_section_id,
                        principalTable: "store_sections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "shopping_list_item_recipe_sources",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    shopping_list_item_id = table.Column<string>(type: "text", nullable: false),
                    recipe_id = table.Column<string>(type: "text", nullable: false),
                    original_quantity = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: true),
                    original_unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shopping_list_item_recipe_sources", x => x.id);
                    table.ForeignKey(
                        name: "fk_shopping_list_item_recipe_sources_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_shopping_list_item_recipe_sources_shopping_list_items_shopp",
                        column: x => x.shopping_list_item_id,
                        principalTable: "shopping_list_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_users_default_store_id",
                table: "users",
                column: "default_store_id");

            migrationBuilder.CreateIndex(
                name: "ix_item_collection_items_item_collection_id",
                table: "item_collection_items",
                column: "item_collection_id");

            migrationBuilder.CreateIndex(
                name: "ix_item_collection_items_store_section_id",
                table: "item_collection_items",
                column: "store_section_id");

            migrationBuilder.CreateIndex(
                name: "ix_item_collections_tenant_id",
                table: "item_collections",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_item_collections_tenant_id_name",
                table: "item_collections",
                columns: new[] { "tenant_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_shopping_list_item_recipe_sources_recipe_id",
                table: "shopping_list_item_recipe_sources",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "ix_shopping_list_item_recipe_sources_shopping_list_item_id",
                table: "shopping_list_item_recipe_sources",
                column: "shopping_list_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_shopping_list_item_recipe_sources_shopping_list_item_id_rec",
                table: "shopping_list_item_recipe_sources",
                columns: new[] { "shopping_list_item_id", "recipe_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_shopping_list_items_shopping_list_id",
                table: "shopping_list_items",
                column: "shopping_list_id");

            migrationBuilder.CreateIndex(
                name: "ix_shopping_list_items_store_section_id",
                table: "shopping_list_items",
                column: "store_section_id");

            migrationBuilder.CreateIndex(
                name: "ix_shopping_lists_store_id",
                table: "shopping_lists",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "ix_shopping_lists_tenant_id",
                table: "shopping_lists",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_store_aisles_store_id",
                table: "store_aisles",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "ix_store_aisles_store_id_store_section_id",
                table: "store_aisles",
                columns: new[] { "store_id", "store_section_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_store_aisles_store_section_id",
                table: "store_aisles",
                column: "store_section_id");

            migrationBuilder.CreateIndex(
                name: "ix_store_default_collections_item_collection_id",
                table: "store_default_collections",
                column: "item_collection_id");

            migrationBuilder.CreateIndex(
                name: "ix_store_default_collections_store_id",
                table: "store_default_collections",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "ix_store_default_collections_store_id_item_collection_id",
                table: "store_default_collections",
                columns: new[] { "store_id", "item_collection_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_store_sections_tenant_id",
                table: "store_sections",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_store_sections_tenant_id_name",
                table: "store_sections",
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
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_users_stores_default_store_id",
                table: "users",
                column: "default_store_id",
                principalTable: "stores",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_stores_default_store_id",
                table: "users");

            migrationBuilder.DropTable(
                name: "item_collection_items");

            migrationBuilder.DropTable(
                name: "shopping_list_item_recipe_sources");

            migrationBuilder.DropTable(
                name: "store_aisles");

            migrationBuilder.DropTable(
                name: "store_default_collections");

            migrationBuilder.DropTable(
                name: "shopping_list_items");

            migrationBuilder.DropTable(
                name: "item_collections");

            migrationBuilder.DropTable(
                name: "shopping_lists");

            migrationBuilder.DropTable(
                name: "store_sections");

            migrationBuilder.DropTable(
                name: "stores");

            migrationBuilder.DropIndex(
                name: "ix_users_default_store_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "default_store_id",
                table: "users");
        }
    }
}
