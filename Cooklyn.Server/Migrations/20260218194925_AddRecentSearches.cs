using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cooklyn.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddRecentSearches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "recent_searches",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    tenant_id = table.Column<string>(type: "text", nullable: false),
                    search_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    search_text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    resource_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    resource_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recent_searches", x => x.id);
                    table.ForeignKey(
                        name: "fk_recent_searches_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_recent_searches_tenant_id_created_on",
                table: "recent_searches",
                columns: new[] { "tenant_id", "created_on" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "ix_recent_searches_tenant_id_search_type_search_text_resource_",
                table: "recent_searches",
                columns: new[] { "tenant_id", "search_type", "search_text", "resource_type", "resource_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "recent_searches");
        }
    }
}
