using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ABCRetail2.Migrations
{
    /// <inheritdoc />
    public partial class addedcontroller : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAccounts",
                table: "UserAccounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerProfiles",
                table: "CustomerProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CartItems",
                table: "CartItems");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAccounts",
                table: "UserAccounts",
                columns: new[] { "PartitionKey", "RowKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                columns: new[] { "PartitionKey", "RowKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders",
                table: "Orders",
                columns: new[] { "PartitionKey", "RowKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerProfiles",
                table: "CustomerProfiles",
                columns: new[] { "PartitionKey", "RowKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CartItems",
                table: "CartItems",
                columns: new[] { "PartitionKey", "RowKey" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAccounts",
                table: "UserAccounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerProfiles",
                table: "CustomerProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CartItems",
                table: "CartItems");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAccounts",
                table: "UserAccounts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders",
                table: "Orders",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerProfiles",
                table: "CustomerProfiles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CartItems",
                table: "CartItems",
                column: "Id");
        }
    }
}
