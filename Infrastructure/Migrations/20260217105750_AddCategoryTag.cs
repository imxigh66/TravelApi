using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "category_tag",
                columns: table => new
                {
                    CategoryTagId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_category_tag", x => x.CategoryTagId);
                });

            migrationBuilder.CreateTable(
                name: "category_tag_link",
                columns: table => new
                {
                    PlaceId = table.Column<int>(type: "int", nullable: false),
                    CategoryTagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_category_tag_link", x => new { x.PlaceId, x.CategoryTagId });
                    table.ForeignKey(
                        name: "FK_category_tag_link_category_tag_CategoryTagId",
                        column: x => x.CategoryTagId,
                        principalTable: "category_tag",
                        principalColumn: "CategoryTagId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_category_tag_link_place_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "place",
                        principalColumn: "PlaceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_category_tag_name",
                table: "category_tag",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_category_tag_link_CategoryTagId",
                table: "category_tag_link",
                column: "CategoryTagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "category_tag_link");

            migrationBuilder.DropTable(
                name: "category_tag");
        }
    }
}
