using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePlaceEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PlaceType",
                table: "place",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdditionalInfo",
                table: "place",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AverageRating",
                table: "place",
                type: "decimal(3,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "BusinessOwnerId",
                table: "place",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "place",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ClaimedAt",
                table: "place",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "place",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsClaimed",
                table: "place",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "place",
                type: "decimal(10,8)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "place",
                type: "decimal(11,8)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReviewsCount",
                table: "place",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SavesCount",
                table: "place",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "place",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ViewsCount",
                table: "place",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_place_active_rating",
                table: "place",
                columns: new[] { "IsActive", "AverageRating" });

            migrationBuilder.CreateIndex(
                name: "IX_place_BusinessOwnerId",
                table: "place",
                column: "BusinessOwnerId");

            migrationBuilder.CreateIndex(
                name: "ix_place_category",
                table: "place",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "ix_place_coordinates",
                table: "place",
                columns: new[] { "Latitude", "Longitude" });

            migrationBuilder.AddForeignKey(
                name: "FK_place_users_BusinessOwnerId",
                table: "place",
                column: "BusinessOwnerId",
                principalTable: "users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_place_users_BusinessOwnerId",
                table: "place");

            migrationBuilder.DropIndex(
                name: "ix_place_active_rating",
                table: "place");

            migrationBuilder.DropIndex(
                name: "IX_place_BusinessOwnerId",
                table: "place");

            migrationBuilder.DropIndex(
                name: "ix_place_category",
                table: "place");

            migrationBuilder.DropIndex(
                name: "ix_place_coordinates",
                table: "place");

            migrationBuilder.DropColumn(
                name: "AdditionalInfo",
                table: "place");

            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "place");

            migrationBuilder.DropColumn(
                name: "BusinessOwnerId",
                table: "place");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "place");

            migrationBuilder.DropColumn(
                name: "ClaimedAt",
                table: "place");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "place");

            migrationBuilder.DropColumn(
                name: "IsClaimed",
                table: "place");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "place");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "place");

            migrationBuilder.DropColumn(
                name: "ReviewsCount",
                table: "place");

            migrationBuilder.DropColumn(
                name: "SavesCount",
                table: "place");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "place");

            migrationBuilder.DropColumn(
                name: "ViewsCount",
                table: "place");

            migrationBuilder.AlterColumn<string>(
                name: "PlaceType",
                table: "place",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }
    }
}
