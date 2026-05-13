using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notification",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecipientId = table.Column<int>(type: "int", nullable: false),
                    ActorId = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Link = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_notification_users_ActorId",
                        column: x => x.ActorId,
                        principalTable: "users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_notification_users_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_notification_ActorId",
                table: "notification",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "ix_notification_created",
                table: "notification",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "ix_notification_recipient",
                table: "notification",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "ix_notification_recipient_unread",
                table: "notification",
                columns: new[] { "RecipientId", "IsRead" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notification");
        }
    }
}
