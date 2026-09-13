using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace lizari.Migrations
{
    /// <inheritdoc />
    public partial class AddMainProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMainProduct",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMainProduct",
                table: "Products");
        }
    }
}
