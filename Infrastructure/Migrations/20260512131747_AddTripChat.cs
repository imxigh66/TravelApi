using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTripChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "trip_message",
                columns: table => new
                {
                    TripMessageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TripId = table.Column<int>(type: "int", nullable: false),
                    SenderId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trip_message", x => x.TripMessageId);
                    table.ForeignKey(
                        name: "FK_trip_message_trip_TripId",
                        column: x => x.TripId,
                        principalTable: "trip",
                        principalColumn: "TripId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trip_message_users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_trip_message_SenderId",
                table: "trip_message",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "ix_trip_message_trip",
                table: "trip_message",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "ix_trip_message_trip_created",
                table: "trip_message",
                columns: new[] { "TripId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trip_message");
        }
    }
}
