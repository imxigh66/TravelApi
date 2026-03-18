using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTripDestinationsAndDayNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DayNumber",
                table: "trip_place",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DestinationId",
                table: "trip_place",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "trip_destination",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TripId = table.Column<int>(type: "int", nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CountryCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    DateFrom = table.Column<DateOnly>(type: "date", nullable: true),
                    DateTo = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trip_destination", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trip_destination_trip_TripId",
                        column: x => x.TripId,
                        principalTable: "trip",
                        principalColumn: "TripId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_trip_place_DestinationId",
                table: "trip_place",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "ix_trip_dest_sort",
                table: "trip_destination",
                columns: new[] { "TripId", "SortOrder" });

            migrationBuilder.AddForeignKey(
                name: "FK_trip_place_trip_destination_DestinationId",
                table: "trip_place",
                column: "DestinationId",
                principalTable: "trip_destination",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trip_place_trip_destination_DestinationId",
                table: "trip_place");

            migrationBuilder.DropTable(
                name: "trip_destination");

            migrationBuilder.DropIndex(
                name: "IX_trip_place_DestinationId",
                table: "trip_place");

            migrationBuilder.DropColumn(
                name: "DayNumber",
                table: "trip_place");

            migrationBuilder.DropColumn(
                name: "DestinationId",
                table: "trip_place");
        }
    }
}
