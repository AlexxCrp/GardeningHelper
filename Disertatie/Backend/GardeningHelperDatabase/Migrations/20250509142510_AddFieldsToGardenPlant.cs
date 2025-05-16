using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GardeningHelperDatabase.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsToGardenPlant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PreviousStatus",
                table: "GardenPlants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StatusChangeReason",
                table: "GardenPlants",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreviousStatus",
                table: "GardenPlants");

            migrationBuilder.DropColumn(
                name: "StatusChangeReason",
                table: "GardenPlants");
        }
    }
}
