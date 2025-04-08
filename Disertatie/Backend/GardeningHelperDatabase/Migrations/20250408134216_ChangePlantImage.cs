using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GardeningHelperDatabase.Migrations
{
    /// <inheritdoc />
    public partial class ChangePlantImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Plants");

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Plants",
                type: "varbinary(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Plants");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Plants",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
