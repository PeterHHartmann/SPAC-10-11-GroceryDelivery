using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroceryDeliveryAPI.Migrations
{
    /// <inheritdoc />
    public partial class DeliveryFinalPrayge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
