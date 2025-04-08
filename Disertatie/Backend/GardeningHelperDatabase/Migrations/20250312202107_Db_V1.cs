using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GardeningHelperDatabase.Migrations
{
    /// <inheritdoc />
    public partial class Db_V1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Actions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TriggerStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    NotificationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Plants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CareInstructions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SunlightRequirements = table.Column<int>(type: "int", maxLength: 200, nullable: false),
                    SoilType = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    GrowthPeriod = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HarvestTime = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MinTemperature = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    MaxTemperature = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    MinHumidity = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    MaxHumidity = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    MinRainfall = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    MaxRainfall = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    MinSoilMoisture = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    MaxSoilMoisture = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    WateringThresholdDays = table.Column<int>(type: "int", nullable: false),
                    WateringThresholdRainfall = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserGardens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    xSize = table.Column<int>(type: "int", nullable: false),
                    ySize = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGardens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserGardens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserWeatherData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Temperature = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Humidity = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Rainfall = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWeatherData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserWeatherData_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserInputs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PlantId = table.Column<int>(type: "int", nullable: false),
                    SoilMoisture = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    GrowthStage = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Observations = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    InputDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInputs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserInputs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserInputs_Plants_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GardenPlants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserGardenId = table.Column<int>(type: "int", nullable: false),
                    PlantId = table.Column<int>(type: "int", nullable: false),
                    PositionX = table.Column<int>(type: "int", nullable: false),
                    PositionY = table.Column<int>(type: "int", nullable: false),
                    DaysToWateringCounter = table.Column<int>(type: "int", nullable: false),
                    LastWateredDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastRainfallDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastRainfallAmount = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    LastSoilMoisture = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    LastStatusCheckDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GardenPlants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GardenPlants_Plants_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GardenPlants_UserGardens_UserGardenId",
                        column: x => x.UserGardenId,
                        principalTable: "UserGardens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GardenPlants_PlantId",
                table: "GardenPlants",
                column: "PlantId");

            migrationBuilder.CreateIndex(
                name: "IX_GardenPlants_UserGardenId",
                table: "GardenPlants",
                column: "UserGardenId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGardens_UserId",
                table: "UserGardens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInputs_PlantId",
                table: "UserInputs",
                column: "PlantId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInputs_UserId",
                table: "UserInputs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWeatherData_UserId",
                table: "UserWeatherData",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Actions");

            migrationBuilder.DropTable(
                name: "GardenPlants");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "UserInputs");

            migrationBuilder.DropTable(
                name: "UserWeatherData");

            migrationBuilder.DropTable(
                name: "UserGardens");

            migrationBuilder.DropTable(
                name: "Plants");
        }
    }
}
