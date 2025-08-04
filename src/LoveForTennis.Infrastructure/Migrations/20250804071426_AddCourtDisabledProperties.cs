using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoveForTennis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCourtDisabledProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "IsDisabledFrom",
                table: "Courts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IsDisabledTo",
                table: "Courts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IsDisabledByUser",
                table: "Courts",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDisabledFrom",
                table: "Courts");

            migrationBuilder.DropColumn(
                name: "IsDisabledTo",
                table: "Courts");

            migrationBuilder.DropColumn(
                name: "IsDisabledByUser",
                table: "Courts");
        }
    }
}