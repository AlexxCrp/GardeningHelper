using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GardeningHelperDatabase.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStatusFromPlant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Plants");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "GardenPlants",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "GardenPlants");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Plants",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
