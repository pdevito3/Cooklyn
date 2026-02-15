using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cooklyn.Server.Migrations
{
    /// <inheritdoc />
    public partial class FixUniqueIndexesForSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_stores_tenant_id_name",
                table: "stores");

            migrationBuilder.DropIndex(
                name: "ix_store_sections_tenant_id_name",
                table: "store_sections");

            migrationBuilder.DropIndex(
                name: "ix_item_collections_tenant_id_name",
                table: "item_collections");

            migrationBuilder.CreateIndex(
                name: "ix_stores_tenant_id_name",
                table: "stores",
                columns: new[] { "tenant_id", "name" },
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_store_sections_tenant_id_name",
                table: "store_sections",
                columns: new[] { "tenant_id", "name" },
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_item_collections_tenant_id_name",
                table: "item_collections",
                columns: new[] { "tenant_id", "name" },
                unique: true,
                filter: "is_deleted = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_stores_tenant_id_name",
                table: "stores");

            migrationBuilder.DropIndex(
                name: "ix_store_sections_tenant_id_name",
                table: "store_sections");

            migrationBuilder.DropIndex(
                name: "ix_item_collections_tenant_id_name",
                table: "item_collections");

            migrationBuilder.CreateIndex(
                name: "ix_stores_tenant_id_name",
                table: "stores",
                columns: new[] { "tenant_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_store_sections_tenant_id_name",
                table: "store_sections",
                columns: new[] { "tenant_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_item_collections_tenant_id_name",
                table: "item_collections",
                columns: new[] { "tenant_id", "name" },
                unique: true);
        }
    }
}
