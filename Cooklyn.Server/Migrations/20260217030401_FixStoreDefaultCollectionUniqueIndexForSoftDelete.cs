using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cooklyn.Server.Migrations
{
    /// <inheritdoc />
    public partial class FixStoreDefaultCollectionUniqueIndexForSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_store_default_collections_store_id_item_collection_id",
                table: "store_default_collections");

            migrationBuilder.CreateIndex(
                name: "ix_store_default_collections_store_id_item_collection_id",
                table: "store_default_collections",
                columns: new[] { "store_id", "item_collection_id" },
                unique: true,
                filter: "is_deleted = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_store_default_collections_store_id_item_collection_id",
                table: "store_default_collections");

            migrationBuilder.CreateIndex(
                name: "ix_store_default_collections_store_id_item_collection_id",
                table: "store_default_collections",
                columns: new[] { "store_id", "item_collection_id" },
                unique: true);
        }
    }
}
