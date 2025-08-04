using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoveForTennis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCourtIdToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourtId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "BookingType",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CourtId",
                table: "Bookings",
                column: "CourtId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Courts_CourtId",
                table: "Bookings",
                column: "CourtId",
                principalTable: "Courts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Courts_CourtId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_CourtId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CourtId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "BookingType",
                table: "Bookings");
        }
    }
}