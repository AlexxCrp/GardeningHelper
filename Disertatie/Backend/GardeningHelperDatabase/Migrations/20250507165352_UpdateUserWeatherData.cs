using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GardeningHelperDatabase.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserWeatherData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "General",
                table: "UserWeatherData",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "General",
                table: "UserWeatherData");
        }
    }
}
