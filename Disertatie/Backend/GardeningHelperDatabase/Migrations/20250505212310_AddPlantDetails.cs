using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GardeningHelperDatabase.Migrations
{
    /// <inheritdoc />
    public partial class AddPlantDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CareInstructions",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Plants");

            migrationBuilder.CreateTable(
                name: "PlantDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlantId = table.Column<int>(type: "int", nullable: false),
                    BloomSeason = table.Column<int>(type: "int", nullable: false),
                    Lifecycle = table.Column<int>(type: "int", nullable: false),
                    WaterNeeds = table.Column<int>(type: "int", nullable: false),
                    DifficultyLevel = table.Column<int>(type: "int", nullable: false),
                    NativeTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdealPhLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GrowingZones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeightAtMaturity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpreadAtMaturity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DaysToGermination = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DaysToMaturity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlantingDepth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpacingBetweenPlants = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Purposes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropagationMethods = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PruningInstructions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PestManagement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiseaseManagement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FertilizationSchedule = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WinterCare = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HarvestingTips = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StorageTips = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CulinaryUses = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MedicinalUses = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HistoricalNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdditionalNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrls = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanionPlantIds = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlantDetails_Plants_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlantDetails_PlantId",
                table: "PlantDetails",
                column: "PlantId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlantDetails");

            migrationBuilder.AddColumn<string>(
                name: "CareInstructions",
                table: "Plants",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Plants",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
