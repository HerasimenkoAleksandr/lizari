using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace lizari.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierProductInformation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SupplierDescription",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SupplierName",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupplierDescription",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SupplierName",
                table: "Products");
        }
    }
}
