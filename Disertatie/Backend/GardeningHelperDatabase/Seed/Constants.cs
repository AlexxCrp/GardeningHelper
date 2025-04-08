﻿namespace GardeningHelperDatabase.Seed
{
    public class Constants
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public const string PlantsJson = @"
        [
            {
            ""Name"": ""Roses"",
            ""Description"": ""Roses are one of the most popular flowers in Romania, known for their beauty and fragrance. They require regular care and pruning."",
            ""CareInstructions"": ""Plant in well-drained soil with full sunlight. Water deeply once a week. Prune in early spring to promote growth."",
            ""SunlightRequirements"": ""FullSun"",
            ""SoilType"": ""Loamy"",
            ""GrowthPeriod"": ""5-9"",
            ""HarvestTime"": ""Spring"",
            ""ImageUrl"": ""https://example.com/roses.jpg"",
            ""MinTemperature"": 5.0,
            ""MaxTemperature"": 30.0,
            ""MinHumidity"": 40.0,
            ""MaxHumidity"": 70.0,
            ""MinRainfall"": 20.0,
            ""MaxRainfall"": 60.0,
            ""MinSoilMoisture"": 30.0,
            ""MaxSoilMoisture"": 60.0,
            ""WateringThresholdDays"": 7,
            ""WateringThresholdRainfall"": 10.0,
            ""Status"": ""Normal""
            },
            {
            ""Name"": ""Tulips"",
            ""Description"": ""Tulips are vibrant spring flowers that thrive in cooler climates. They are popular in Romanian gardens for their bright colors."",
            ""CareInstructions"": ""Plant bulbs in well-drained soil in fall. Water moderately during growth. Remove spent flowers to encourage bulb growth."",
            ""SunlightRequirements"": ""FullSun"",
            ""SoilType"": ""Sandy"",
            ""GrowthPeriod"": ""3-6"",
            ""HarvestTime"": ""Spring"",
            ""ImageUrl"": ""https://example.com/tulips.jpg"",
            ""MinTemperature"": 0.0,
            ""MaxTemperature"": 25.0,
            ""MinHumidity"": 30.0,
            ""MaxHumidity"": 60.0,
            ""MinRainfall"": 15.0,
            ""MaxRainfall"": 50.0,
            ""MinSoilMoisture"": 20.0,
            ""MaxSoilMoisture"": 50.0,
            ""WateringThresholdDays"": 5,
            ""WateringThresholdRainfall"": 5.0,
            ""Status"": ""Normal""
            },
            {
            ""Name"": ""Lavender"",
            ""Description"": ""Lavender is a fragrant herb that thrives in sunny, dry conditions. It is popular in Romania for its aroma and medicinal properties."",
            ""CareInstructions"": ""Plant in well-drained soil with full sunlight. Water sparingly. Prune after flowering to maintain shape."",
            ""SunlightRequirements"": ""FullSun"",
            ""SoilType"": ""Sandy"",
            ""GrowthPeriod"": ""5-9"",
            ""HarvestTime"": ""Summer"",
            ""ImageUrl"": ""https://example.com/lavender.jpg"",
            ""MinTemperature"": 10.0,
            ""MaxTemperature"": 35.0,
            ""MinHumidity"": 20.0,
            ""MaxHumidity"": 50.0,
            ""MinRainfall"": 10.0,
            ""MaxRainfall"": 40.0,
            ""MinSoilMoisture"": 15.0,
            ""MaxSoilMoisture"": 40.0,
            ""WateringThresholdDays"": 10,
            ""WateringThresholdRainfall"": 5.0,
            ""Status"": ""Normal""
            },
            {
            ""Name"": ""Tomatoes"",
            ""Description"": ""Tomatoes are a staple in Romanian gardens. They require regular watering and sunlight to produce juicy fruits."",
            ""CareInstructions"": ""Plant in well-drained soil with full sunlight. Water regularly, especially during fruiting. Support with stakes or cages."",
            ""SunlightRequirements"": ""FullSun"",
            ""SoilType"": ""Loamy"",
            ""GrowthPeriod"": ""5-9"",
            ""HarvestTime"": ""Summer"",
            ""ImageUrl"": ""https://example.com/tomatoes.jpg"",
            ""MinTemperature"": 15.0,
            ""MaxTemperature"": 30.0,
            ""MinHumidity"": 40.0,
            ""MaxHumidity"": 70.0,
            ""MinRainfall"": 30.0,
            ""MaxRainfall"": 70.0,
            ""MinSoilMoisture"": 40.0,
            ""MaxSoilMoisture"": 70.0,
            ""WateringThresholdDays"": 3,
            ""WateringThresholdRainfall"": 10.0,
            ""Status"": ""Normal""
            },
            {
            ""Name"": ""Peppers"",
            ""Description"": ""Peppers are a popular vegetable in Romania, known for their versatility in cooking. They thrive in warm, sunny conditions."",
            ""CareInstructions"": ""Plant in well-drained soil with full sunlight. Water regularly. Mulch to retain soil moisture."",
            ""SunlightRequirements"": ""FullSun"",
            ""SoilType"": ""Loamy"",
            ""GrowthPeriod"": ""5-9"",
            ""HarvestTime"": ""Summer"",
            ""ImageUrl"": ""https://example.com/peppers.jpg"",
            ""MinTemperature"": 18.0,
            ""MaxTemperature"": 32.0,
            ""MinHumidity"": 40.0,
            ""MaxHumidity"": 70.0,
            ""MinRainfall"": 25.0,
            ""MaxRainfall"": 60.0,
            ""MinSoilMoisture"": 35.0,
            ""MaxSoilMoisture"": 65.0,
            ""WateringThresholdDays"": 4,
            ""WateringThresholdRainfall"": 10.0,
            ""Status"": ""Normal""
            },
            {
            ""Name"": ""Cucumbers"",
            ""Description"": ""Cucumbers are a refreshing vegetable that grows well in Romanian gardens. They require consistent watering and support for climbing."",
            ""CareInstructions"": ""Plant in well-drained soil with full sunlight. Water regularly. Provide trellises for climbing varieties."",
            ""SunlightRequirements"": ""FullSun"",
            ""SoilType"": ""Loamy"",
            ""GrowthPeriod"": ""5-9"",
            ""HarvestTime"": ""Summer"",
            ""ImageUrl"": ""https://example.com/cucumbers.jpg"",
            ""MinTemperature"": 16.0,
            ""MaxTemperature"": 30.0,
            ""MinHumidity"": 50.0,
            ""MaxHumidity"": 80.0,
            ""MinRainfall"": 30.0,
            ""MaxRainfall"": 70.0,
            ""MinSoilMoisture"": 40.0,
            ""MaxSoilMoisture"": 70.0,
            ""WateringThresholdDays"": 2,
            ""WateringThresholdRainfall"": 10.0,
            ""Status"": ""Normal""
            },
            {
            ""Name"": ""Carrots"",
            ""Description"": ""Carrots are a root vegetable that grows well in Romanian soil. They require loose, sandy soil for proper root development."",
            ""CareInstructions"": ""Plant in loose, sandy soil with full sunlight. Water regularly. Thin seedlings to prevent overcrowding."",
            ""SunlightRequirements"": ""FullSun"",
            ""SoilType"": ""Sandy"",
            ""GrowthPeriod"": ""4-9"",
            ""HarvestTime"": ""Fall"",
            ""ImageUrl"": ""https://example.com/carrots.jpg"",
            ""MinTemperature"": 10.0,
            ""MaxTemperature"": 25.0,
            ""MinHumidity"": 40.0,
            ""MaxHumidity"": 70.0,
            ""MinRainfall"": 20.0,
            ""MaxRainfall"": 50.0,
            ""MinSoilMoisture"": 30.0,
            ""MaxSoilMoisture"": 60.0,
            ""WateringThresholdDays"": 5,
            ""WateringThresholdRainfall"": 10.0,
            ""Status"": ""Normal""
            },
            {
            ""Name"": ""Potatoes"",
            ""Description"": ""Potatoes are a staple crop in Romania. They grow best in cool climates and require well-drained soil."",
            ""CareInstructions"": ""Plant in well-drained soil with full sunlight. Water regularly. Hill soil around plants to protect tubers."",
            ""SunlightRequirements"": ""FullSun"",
            ""SoilType"": ""Loamy"",
            ""GrowthPeriod"": ""4-9"",
            ""HarvestTime"": ""Fall"",
            ""ImageUrl"": ""https://example.com/potatoes.jpg"",
            ""MinTemperature"": 10.0,
            ""MaxTemperature"": 25.0,
            ""MinHumidity"": 40.0,
            ""MaxHumidity"": 70.0,
            ""MinRainfall"": 30.0,
            ""MaxRainfall"": 60.0,
            ""MinSoilMoisture"": 35.0,
            ""MaxSoilMoisture"": 65.0,
            ""WateringThresholdDays"": 5,
            ""WateringThresholdRainfall"": 10.0,
            ""Status"": ""Normal""
            }
        ]";
    }
}
