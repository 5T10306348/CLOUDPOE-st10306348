using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ABCRetail2.Migrations
{
    /// <inheritdoc />
    public partial class registermethodguidnotworkinghadtoswitchtosomethingelse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "UserAccounts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "UserAccounts");
        }
    }
}
