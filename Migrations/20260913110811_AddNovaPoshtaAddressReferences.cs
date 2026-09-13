using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace lizari.Migrations
{
    /// <inheritdoc />
    public partial class AddNovaPoshtaAddressReferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CityRef",
                table: "Orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WarehouseRef",
                table: "Orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CityRef",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "WarehouseRef",
                table: "Orders");
        }
    }
}
