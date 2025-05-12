using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GroceryDeliveryAPI.Migrations
{
    /// <inheritdoc />
    public partial class delivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create a temporary column for DeliveryPersons
            migrationBuilder.AddColumn<int>(
                name: "Status_Int",
                table: "DeliveryPersons",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // Convert string values to integers based on enum positions
            migrationBuilder.Sql(@"
                UPDATE ""DeliveryPersons"" SET ""Status_Int"" = 
                CASE ""Status""
                    WHEN 'Available' THEN 0
                    WHEN 'Busy' THEN 1
                    WHEN 'OnBreak' THEN 2
                    WHEN 'Offline' THEN 3
                    WHEN 'Inactive' THEN 4
                    ELSE 0
                END");

            // Drop the old column
            migrationBuilder.DropColumn(
                name: "Status",
                table: "DeliveryPersons");

            // Rename the temporary column to the original name
            migrationBuilder.RenameColumn(
                name: "Status_Int",
                table: "DeliveryPersons",
                newName: "Status");

            // Create a temporary column for Deliveries
            migrationBuilder.AddColumn<int>(
                name: "Status_Int",
                table: "Deliveries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // Convert string values to integers based on enum positions
            migrationBuilder.Sql(@"
                UPDATE ""Deliveries"" SET ""Status_Int"" = 
                CASE ""Status""
                    WHEN 'Pending' THEN 0
                    WHEN 'InProgress' THEN 1
                    WHEN 'Completed' THEN 2
                    WHEN 'Cancelled' THEN 3
                    ELSE 0
                END");

            // Drop the old column
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Deliveries");

            // Rename the temporary column to the original name
            migrationBuilder.RenameColumn(
                name: "Status_Int",
                table: "Deliveries",
                newName: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Create a temporary column for DeliveryPersons
            migrationBuilder.AddColumn<string>(
                name: "Status_Str",
                table: "DeliveryPersons",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Available");

            // Convert integer values back to strings
            migrationBuilder.Sql(@"
                UPDATE ""DeliveryPersons"" SET ""Status_Str"" = 
                CASE ""Status""
                    WHEN 0 THEN 'Available'
                    WHEN 1 THEN 'Busy'
                    WHEN 2 THEN 'OnBreak'
                    WHEN 3 THEN 'Offline'
                    WHEN 4 THEN 'Inactive'
                    ELSE 'Available'
                END");

            // Drop the old column
            migrationBuilder.DropColumn(
                name: "Status",
                table: "DeliveryPersons");

            // Rename the temporary column to the original name
            migrationBuilder.RenameColumn(
                name: "Status_Str",
                table: "DeliveryPersons",
                newName: "Status");

            // Create a temporary column for Deliveries
            migrationBuilder.AddColumn<string>(
                name: "Status_Str",
                table: "Deliveries",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pending");

            // Convert integer values back to strings
            migrationBuilder.Sql(@"
                UPDATE ""Deliveries"" SET ""Status_Str"" = 
                CASE ""Status""
                    WHEN 0 THEN 'Pending'
                    WHEN 1 THEN 'InProgress'
                    WHEN 2 THEN 'Completed'
                    WHEN 3 THEN 'Cancelled'
                    ELSE 'Pending'
                END");

            // Drop the old column
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Deliveries");

            // Rename the temporary column to the original name
            migrationBuilder.RenameColumn(
                name: "Status_Str",
                table: "Deliveries",
                newName: "Status");
        }
    }
}
