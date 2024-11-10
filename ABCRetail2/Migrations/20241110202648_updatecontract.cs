using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ABCRetail2.Migrations
{
    /// <inheritdoc />
    public partial class updatecontract : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileContent",
                table: "Contracts",
                newName: "FileData");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Contracts");

            migrationBuilder.RenameColumn(
                name: "FileData",
                table: "Contracts",
                newName: "FileContent");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "Contracts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
