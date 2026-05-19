using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventBookingSystem.Migrations
{
    /// <inheritdoc />
    public partial class eventBookingDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    event_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    location = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    total_seats = table.Column<int>(type: "int", nullable: false),
                    available_seats = table.Column<int>(type: "int", nullable: false),
                    image_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.event_id);
                });

            migrationBuilder.CreateTable(
                name: "USERS",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    role = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    phone = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USERS", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "Booking",
                columns: table => new
                {
                    booking_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    event_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    booking_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    number_of_tickets = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Booking", x => x.booking_id);
                    table.ForeignKey(
                        name: "FK_Booking_Event_event_id",
                        column: x => x.event_id,
                        principalTable: "Event",
                        principalColumn: "event_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Booking_USERS_user_id",
                        column: x => x.user_id,
                        principalTable: "USERS",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Booking_event_id",
                table: "Booking",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_user_id",
                table: "Booking",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_USERS_email",
                table: "USERS",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Booking");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "USERS");
        }
    }
}
