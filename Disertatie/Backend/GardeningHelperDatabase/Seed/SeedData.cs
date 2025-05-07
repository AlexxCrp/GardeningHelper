using DataExchange.Enums;
using GardeningHelperDatabase.Entities;
using GardeningHelperDatabase.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GardeningHelperDatabase.Seed
{
    public class SeedData
    {
        public static async Task SeedDatabase(GardeningHelperDbContext context, RoleManager<Role> roleManager)
        {
            await SeedRoles(roleManager);
            SeedPlants(context);
            SeedPlantDetails(context);
            context.SaveChanges();
        }

        public static void SeedPlants(GardeningHelperDbContext context)
        {
            if (context.Plants.Any())
            {
                return;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };

            var plants = JsonSerializer.Deserialize<List<Plant>>(Constants.PlantsJson, options);

            // Get the path to the PlantImages folder
            string plantImagesFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "..\\GardeningHelperDatabase\\Seed\\PlantImages");

            // Iterate over the plants and load their respective images
            foreach (var plant in plants)
            {
                // Construct the path to the image file for the current plant
                string imageFilePath = Path.Combine(plantImagesFolderPath, $"{plant.Name}.png");

                if (File.Exists(imageFilePath))
                {
                    // Read the image file and convert it to a byte array
                    byte[] imageBytes = File.ReadAllBytes(imageFilePath);
                    plant.Image = imageBytes;
                }
                else
                {
                    // You can decide to set a default image or handle this case differently
                    plant.Image = null; // Or load a default image if needed
                }
            }

            context.Plants.AddRange(plants);
            context.SaveChanges();
        }

        public static async Task SeedRoles(RoleManager<Role> roleManager)
        {
            // Check if the roles already exist
            if (await roleManager.RoleExistsAsync(Constants.Admin))
            {
                return; // Roles have already been seeded
            }

            // Create and add the Admin role
            var adminRole = new Role { Name = Constants.Admin };
            await roleManager.CreateAsync(adminRole);

            // Create and add the User role
            var userRole = new Role { Name = Constants.User };
            await roleManager.CreateAsync(userRole);
        }

        public static void SeedPlantDetails(GardeningHelperDbContext context)
        {
            if (context.PlantDetails.Any())
            {
                return;
            }

            // Get all plants to establish relationships
            var plants = context.Plants.ToList();
            var plantDetails = new List<PlantDetails>();

            // Rose details
            var roseDetails = new PlantDetails
            {
                PlantId = plants.First(p => p.Name == "Rose").Id,
                BloomSeason = BloomSeasonEnum.SpringSummer,
                Lifecycle = PlantLifecycleEnum.Perennial,
                WaterNeeds = WaterNeedsEnum.Medium,
                DifficultyLevel = DifficultyLevelEnum.Intermediate,
                NativeTo = "Asia, Europe, North America",
                IdealPhLevel = "6.0-6.5",
                GrowingZones = "5-9",
                HeightAtMaturity = "1-2 meters (depending on variety)",
                SpreadAtMaturity = "0.5-1 meter",
                DaysToGermination = "14-21 days",
                DaysToMaturity = "120-180 days to bloom",
                PlantingDepth = "5-10 cm for bare root plants",
                SpacingBetweenPlants = "60-90 cm",
                Purposes = new List<PlantPurposeEnum> { PlantPurposeEnum.Ornamental, PlantPurposeEnum.Aromatic },
                PropagationMethods = "Cuttings, grafting, budding",
                PruningInstructions = "Prune in early spring before new growth starts. Remove dead, diseased, or crossing branches. Cut back to an outward-facing bud at a 45-degree angle.",
                PestManagement = "Watch for aphids, Japanese beetles, and rose slugs. Treat with neem oil or insecticidal soap for minor infestations.",
                DiseaseManagement = "Common diseases include black spot, powdery mildew, and rust. Ensure good air circulation and avoid wetting foliage when watering.",
                FertilizationSchedule = "Apply balanced rose fertilizer monthly from spring through summer. Stop fertilizing 6-8 weeks before first frost.",
                WinterCare = "In colder regions, mound soil around the base and cover with mulch. Prune longer canes to prevent wind damage.",
                HarvestingTips = "Cut roses in the morning when temperatures are cool. Choose buds that are just beginning to open. Cut stems at a 45-degree angle.",
                StorageTips = "Place cut roses in water immediately. Remove lower leaves that would be submerged. Change water daily and recut stems.",
                CulinaryUses = "Rose petals can be used in teas, jams, and as garnishes. Rose hips are rich in vitamin C and can be used for tea.",
                MedicinalUses = "Rose oil has anti-inflammatory properties. Rose water is used as a skin toner and for eye treatments.",
                HistoricalNotes = "Roses have been cultivated for over 5,000 years. They have been symbols of love, beauty, and politics throughout history.",
                AdditionalNotes = "Romania has a strong tradition of rose cultivation, particularly for rose oil production.",
                ImageUrls = new List<string> {
                    "https://m.media-amazon.com/images/I/81Z2i9cyI9L._AC_UF1000,1000_QL80_.jpg",
                    "https://starrosesandplants.com/app/uploads/2024/04/RubyRed_Habit_009-1.jpg",
                    "https://plantura.garden/uk/wp-content/uploads/sites/2/2022/06/planting-rose-bushes.jpg"
                },
                CompanionPlantIds = new List<int> { plants.First(p => p.Name == "Lavender").Id }
            };
            plantDetails.Add(roseDetails);

            // Tulip details
            var tulipDetails = new PlantDetails
            {
                PlantId = plants.First(p => p.Name == "Tulip").Id,
                BloomSeason = BloomSeasonEnum.Spring,
                Lifecycle = PlantLifecycleEnum.Perennial,
                WaterNeeds = WaterNeedsEnum.Medium,
                DifficultyLevel = DifficultyLevelEnum.Beginner,
                NativeTo = "Central Asia, Turkey",
                IdealPhLevel = "6.0-7.0",
                GrowingZones = "3-8",
                HeightAtMaturity = "15-60 cm (depending on variety)",
                SpreadAtMaturity = "10-15 cm",
                DaysToGermination = "Bulbs, not seeds",
                DaysToMaturity = "90-120 days from planting to bloom",
                PlantingDepth = "15-20 cm deep",
                SpacingBetweenPlants = "10-15 cm",
                Purposes = new List<PlantPurposeEnum> { PlantPurposeEnum.Ornamental },
                PropagationMethods = "Bulb division, bulblets",
                PruningInstructions = "Remove spent flowers to prevent seed formation and direct energy back to the bulb. Let foliage die back naturally.",
                PestManagement = "Protect from squirrels, mice, and voles that eat bulbs. Watch for aphids and thrips on foliage and flowers.",
                DiseaseManagement = "Susceptible to tulip fire, botrytis, and bulb rot. Plant in well-draining soil and avoid waterlogged conditions.",
                FertilizationSchedule = "Apply bulb fertilizer at planting time and again when shoots emerge in spring.",
                WinterCare = "No special care needed as tulips require a cold period. In warmer regions, dig up bulbs and chill them.",
                HarvestingTips = "Cut when buds are colored but not fully open. Cut in the morning and place in water immediately.",
                StorageTips = "For cut flowers, recut stems and place in cool water. For bulbs, store in a cool, dry place after foliage has died back.",
                CulinaryUses = "Historically, tulip bulbs were eaten during the Dutch famine. Not commonly used in modern cuisine.",
                MedicinalUses = "Limited medicinal use. Some traditional applications for coughs and fever.",
                HistoricalNotes = "Caused 'tulip mania' in the Netherlands in the 1630s, one of the first recorded economic bubbles.",
                AdditionalNotes = "Tulips continue to bloom for 3-5 years before needing replacement. In Romania, they're popular for spring gardens.",
                ImageUrls = new List<string> {
                    "https://cdn.mos.cms.futurecdn.net/eetaEt8RhhsLsfxLBTLwxb.jpg",
                    "https://blog.dutchbulbs.com/wp-content/uploads/2020/09/Tulip_Field_1739-1.jpg",
                    "https://cdn.shopify.com/s/files/1/0267/8236/7792/files/plantingtulips.jpg?v=1568612360"
                },
                CompanionPlantIds = new List<int> { }
            };
            plantDetails.Add(tulipDetails);

            // Lavender details
            var lavenderDetails = new PlantDetails
            {
                PlantId = plants.First(p => p.Name == "Lavender").Id,
                BloomSeason = BloomSeasonEnum.Summer,
                Lifecycle = PlantLifecycleEnum.Perennial,
                WaterNeeds = WaterNeedsEnum.Low,
                DifficultyLevel = DifficultyLevelEnum.Intermediate,
                NativeTo = "Mediterranean region",
                IdealPhLevel = "6.5-7.5",
                GrowingZones = "5-9",
                HeightAtMaturity = "30-90 cm (depending on variety)",
                SpreadAtMaturity = "30-60 cm",
                DaysToGermination = "14-21 days",
                DaysToMaturity = "90-200 days to bloom (first season)",
                PlantingDepth = "Seeds: 0.6 cm, Plants: same depth as nursery pot",
                SpacingBetweenPlants = "30-90 cm (depending on variety)",
                Purposes = new List<PlantPurposeEnum> { PlantPurposeEnum.Ornamental, PlantPurposeEnum.Aromatic, PlantPurposeEnum.Medicinal, PlantPurposeEnum.Companion },
                PropagationMethods = "Seeds, cuttings, division",
                PruningInstructions = "Prune after flowering or in early spring. Cut back by about one-third, but never cut into woody stems.",
                PestManagement = "Generally pest-resistant. Watch for spittlebugs and whiteflies. Can be treated with insecticidal soap.",
                DiseaseManagement = "Susceptible to root rot in wet conditions. Ensure good air circulation to prevent fungal diseases.",
                FertilizationSchedule = "Light feeder. Apply small amount of balanced organic fertilizer in spring. Too much fertilizer reduces essential oil production.",
                WinterCare = "In colder regions, mulch around base but not directly against stems. Avoid winter pruning.",
                HarvestingTips = "Harvest when flowers are just beginning to open for maximum oil content. Cut in the morning after dew has dried.",
                StorageTips = "Hang bundles upside down in a dark, dry area with good air circulation. Store dried lavender in airtight containers.",
                CulinaryUses = "Used in herbes de Provence, flavoring for desserts, lavender sugar, and lavender honey.",
                MedicinalUses = "Used for anxiety, insomnia, and headaches. Lavender oil has antiseptic and anti-inflammatory properties.",
                HistoricalNotes = "The name comes from the Latin 'lavare' meaning 'to wash,' as it was used in Roman baths.",
                AdditionalNotes = "Lavender is growing in popularity in Romanian gardens, especially in drier regions.",
                ImageUrls = new List<string> {
                    "https://cdn.wikifarmer.com/images/thumbnail/2017/06/Lavender-Plant-Wiki-1200x630.jpg",
                    "https://www.greatgardenplants.com/cdn/shop/products/hidcote-lavender-22-sw.jpg?v=1725913331",
                    "https://images.squarespace-cdn.com/content/v1/5c6f81f08155121249a4088c/3706f645-c13e-4cff-8c52-545263e6c14a/IMG_9630_VSCO.JPG"
                },
                CompanionPlantIds = new List<int> { plants.First(p => p.Name == "Rose").Id }
            };
            plantDetails.Add(lavenderDetails);

            // Tomato details
            var tomatoDetails = new PlantDetails
            {
                PlantId = plants.First(p => p.Name == "Tomato").Id,
                BloomSeason = BloomSeasonEnum.Summer,
                Lifecycle = PlantLifecycleEnum.Annual,
                WaterNeeds = WaterNeedsEnum.Medium,
                DifficultyLevel = DifficultyLevelEnum.Beginner,
                NativeTo = "Western South America",
                IdealPhLevel = "6.0-6.8",
                GrowingZones = "4-11",
                HeightAtMaturity = "60-300 cm (depending on variety)",
                SpreadAtMaturity = "60-90 cm",
                DaysToGermination = "5-10 days",
                DaysToMaturity = "60-100 days from transplant to harvest",
                PlantingDepth = "Seeds: 0.6 cm, Seedlings: up to first set of leaves",
                SpacingBetweenPlants = "45-90 cm (depending on variety)",
                Purposes = new List<PlantPurposeEnum> { PlantPurposeEnum.Edible },
                PropagationMethods = "Seeds, cuttings can root in water",
                PruningInstructions = "Remove suckers (side shoots that form in leaf axils) for indeterminate varieties. Prune lower leaves as plant grows to improve air circulation.",
                PestManagement = "Watch for tomato hornworms, aphids, and whiteflies. Handpick larger pests and use insecticidal soap for smaller ones.",
                DiseaseManagement = "Susceptible to blight, wilt, and viral diseases. Rotate crops, avoid wetting foliage, and choose resistant varieties.",
                FertilizationSchedule = "Apply balanced fertilizer at planting time. Side-dress with compost when fruit begins to set. Avoid high nitrogen.",
                WinterCare = "Annual in most regions. Save seeds from heirloom varieties. Clean up all plant material at season end.",
                HarvestingTips = "Harvest when fully colored but still firm. Cut or gently twist fruits from the vine.",
                StorageTips = "Store at room temperature, not in refrigerator. To ripen, place in paper bag with a banana. Freeze, can, or dry for long-term storage.",
                CulinaryUses = "Used fresh in salads, sandwiches, sauces, soups, pastes, juices, and as a base for many Romanian dishes.",
                MedicinalUses = "Rich in lycopene, an antioxidant linked to reduced risk of heart disease and certain cancers.",
                HistoricalNotes = "Originally thought to be poisonous when introduced to Europe. Became popular in Italian cuisine in the 1800s.",
                AdditionalNotes = "One of the most popular garden vegetables in Romania. Many local varieties are cultivated.",
                ImageUrls = new List<string> {
                    "https://www.greenlife.co.ke/wp-content/uploads/2022/04/Tomatoes-Farm-scaled-1.jpg",
                    "https://njaes.rutgers.edu/fs678/FS678-6-big.jpg",
                    "https://www.southernliving.com/thmb/8sJLpOMVrdM3RO6GeyuSVAJa9G8=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/GettyImages-1365178498-81dd069cd1514e288e68516bc96df8d4.jpg"
                },
                CompanionPlantIds = new List<int> { plants.First(p => p.Name == "Carrot").Id, plants.First(p => p.Name == "Cucumber").Id }
            };
            plantDetails.Add(tomatoDetails);

            // Pepper details
            var pepperDetails = new PlantDetails
            {
                PlantId = plants.First(p => p.Name == "Pepper").Id,
                BloomSeason = BloomSeasonEnum.Summer,
                Lifecycle = PlantLifecycleEnum.Annual,
                WaterNeeds = WaterNeedsEnum.Medium,
                DifficultyLevel = DifficultyLevelEnum.Beginner,
                NativeTo = "Central and South America",
                IdealPhLevel = "6.0-6.8",
                GrowingZones = "4-11",
                HeightAtMaturity = "45-75 cm (depending on variety)",
                SpreadAtMaturity = "45-60 cm",
                DaysToGermination = "7-14 days",
                DaysToMaturity = "60-90 days from transplant to harvest",
                PlantingDepth = "Seeds: 0.6 cm, Seedlings: same depth as nursery pot",
                SpacingBetweenPlants = "45-60 cm",
                Purposes = new List<PlantPurposeEnum> { PlantPurposeEnum.Edible, PlantPurposeEnum.Ornamental },
                PropagationMethods = "Seeds",
                PruningInstructions = "Pinch early flowers to encourage stronger plant growth. Remove lower leaves that touch the soil.",
                PestManagement = "Watch for aphids, flea beetles, and pepper weevils. Use row covers early in season and insecticidal soap for infestations.",
                DiseaseManagement = "Susceptible to bacterial spot, phytophthora, and viruses. Plant in well-draining soil and practice crop rotation.",
                FertilizationSchedule = "Apply balanced fertilizer at planting time. Side-dress with compost or balanced fertilizer when first fruits set.",
                WinterCare = "Annual in most regions. Can be overwintered indoors in bright, warm locations.",
                HarvestingTips = "Cut peppers from the plant using scissors or pruners. Can be harvested green or allowed to ripen to full color.",
                StorageTips = "Store unwashed peppers in the refrigerator for up to two weeks. Can be frozen, dried, or pickled for long-term storage.",
                CulinaryUses = "Used in many Romanian dishes, stuffed peppers are particularly popular. Used both fresh and cooked.",
                MedicinalUses = "Capsaicin in hot peppers has pain-relieving properties. Bell peppers are high in vitamin C.",
                HistoricalNotes = "Columbus brought peppers to Europe, mistakenly calling them 'peppers' thinking they were related to black pepper.",
                AdditionalNotes = "Romanian cuisine features many traditional pepper dishes, including zacuscă, a popular vegetable spread.",
                ImageUrls = new List<string> {
                    "https://www.easytogrowbulbs.com/cdn/shop/products/PepperSweetMini_square_SHUT.jpg?v=1612301570&width=1445",
                    "https://www.thespruce.com/thmb/utzJXf3qABMdr_DJAbxOkZ0tRmI=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/GettyImages-1323318476-ea577b06f3f5437da3999be527c458dc.jpg",
                    "https://www.rhs.org.uk/getmedia/f5aca001-eba2-488f-bc37-eb5ce206cffd/sweet-pepper-mohawk-crop-webuse-PUB0020813.jpg?width=624&height=403&ext=.jpg"
                },
                CompanionPlantIds = new List<int> { plants.First(p => p.Name == "Tomato").Id }
            };
            plantDetails.Add(pepperDetails);

            // Cucumber details
            var cucumberDetails = new PlantDetails
            {
                PlantId = plants.First(p => p.Name == "Cucumber").Id,
                BloomSeason = BloomSeasonEnum.Summer,
                Lifecycle = PlantLifecycleEnum.Annual,
                WaterNeeds = WaterNeedsEnum.High,
                DifficultyLevel = DifficultyLevelEnum.Beginner,
                NativeTo = "India",
                IdealPhLevel = "6.0-7.0",
                GrowingZones = "4-11",
                HeightAtMaturity = "15-20 cm (bush types), 180-240 cm (vining types)",
                SpreadAtMaturity = "45-90 cm (bush types), 90-180 cm (vining types)",
                DaysToGermination = "3-10 days",
                DaysToMaturity = "50-70 days from planting to harvest",
                PlantingDepth = "2.5 cm",
                SpacingBetweenPlants = "45-90 cm (depending on variety)",
                Purposes = new List<PlantPurposeEnum> { PlantPurposeEnum.Edible },
                PropagationMethods = "Seeds direct sown after danger of frost",
                PruningInstructions = "Pinch out the growing tips when plants reach 60 cm to encourage branching. Remove yellowing leaves.",
                PestManagement = "Watch for cucumber beetles, aphids, and spider mites. Use row covers until flowering, then remove for pollination.",
                DiseaseManagement = "Susceptible to powdery mildew, bacterial wilt, and mosaic virus. Plant resistant varieties and practice crop rotation.",
                FertilizationSchedule = "Apply balanced fertilizer at planting time. Side-dress with compost or balanced fertilizer when plants begin to vine.",
                WinterCare = "Annual in all regions. Clean up all plant material at season end to prevent disease carryover.",
                HarvestingTips = "Harvest frequently when cucumbers reach desired size but before they yellow. Cut from vine with scissors or pruners.",
                StorageTips = "Store in refrigerator for up to a week. Can be pickled for long-term storage.",
                CulinaryUses = "Used fresh in salads, sandwiches, and tzatziki. Popular pickled in Romanian cuisine.",
                MedicinalUses = "Has cooling properties. Used topically for skin irritations and puffy eyes.",
                HistoricalNotes = "Cultivated for at least 3,000 years. Mentioned in the Epic of Gilgamesh.",
                AdditionalNotes = "Romanian gardens often grow both slicing and pickling varieties. Traditional pickled cucumber recipes are passed down through generations.",
                ImageUrls = new List<string> {
                    "https://www.marthastewart.com/thmb/zfUvh7wPmQNc2RX87hAlvdtgWx4=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/ms-planting-cucumbers-5e12118076a54b4a9e487c92b991de6a.jpg",
                    "https://www.epicgardening.com/wp-content/uploads/2023/09/Cucumbers-growing-in-garden-with-yellow-flowers.jpg",
                    "https://cdn.shopify.com/s/files/1/0603/4892/4151/files/Spacemaster-Cucumber.jpg"
                },
                CompanionPlantIds = new List<int> { plants.First(p => p.Name == "Tomato").Id }
            };
            plantDetails.Add(cucumberDetails);

            // Carrot details
            var carrotDetails = new PlantDetails
            {
                PlantId = plants.First(p => p.Name == "Carrot").Id,
                BloomSeason = BloomSeasonEnum.Summer,
                Lifecycle = PlantLifecycleEnum.Biennial,
                WaterNeeds = WaterNeedsEnum.Medium,
                DifficultyLevel = DifficultyLevelEnum.Beginner,
                NativeTo = "Central Asia and Middle East",
                IdealPhLevel = "6.0-6.8",
                GrowingZones = "3-10",
                HeightAtMaturity = "30-45 cm (foliage)",
                SpreadAtMaturity = "7-10 cm (root diameter)",
                DaysToGermination = "10-21 days",
                DaysToMaturity = "60-80 days from planting to harvest",
                PlantingDepth = "0.6-1.3 cm",
                SpacingBetweenPlants = "5-7 cm (after thinning)",
                Purposes = new List<PlantPurposeEnum> { PlantPurposeEnum.Edible, PlantPurposeEnum.Companion },
                PropagationMethods = "Seeds only, direct sown",
                PruningInstructions = "Thin seedlings to 5-7 cm apart when they reach 5 cm in height. No other pruning necessary.",
                PestManagement = "Watch for carrot rust flies, aphids, and leafhoppers. Use row covers to protect from pests.",
                DiseaseManagement = "Susceptible to aster yellows, leaf blight, and root rot. Practice crop rotation and ensure good air circulation.",
                FertilizationSchedule = "Light feeder. Avoid fresh manure and excess nitrogen which causes forked roots. Apply balanced fertilizer at planting time.",
                WinterCare = "Can be left in ground in mild winter areas with mulch protection. Otherwise, harvest before ground freezes.",
                HarvestingTips = "Harvest when roots reach desired size. Loosen soil around carrots before pulling to avoid breakage.",
                StorageTips = "Remove tops before storing. Store in refrigerator for 2-3 weeks or in sand in a cool root cellar for months.",
                CulinaryUses = "Used raw in salads or as snacks. Cooked in soups, stews, and side dishes. Popular in Romanian borscht.",
                MedicinalUses = "Rich in beta-carotene for eye health. Has antioxidant properties.",
                HistoricalNotes = "Originally purple or yellow. Orange carrots were developed in the Netherlands in the 17th century to honor the House of Orange.",
                AdditionalNotes = "Romanian gardens typically grow orange varieties, but there is increasing interest in heritage purple and yellow varieties.",
                ImageUrls = new List<string> {
                    "https://www.botanicalinterests.com/community/blog/wp-content/uploads/2024/08/carrot-sow-and-grow.jpg",
                    "https://plantura.garden/uk/wp-content/uploads/sites/2/2021/06/growing-carrots-1024x683.jpg?x63657",
                    "https://www.seedparade.co.uk/news/wp-content/uploads/2023/08/harvesting-carrots.jpg"
                },
                CompanionPlantIds = new List<int> { plants.First(p => p.Name == "Tomato").Id }
            };
            plantDetails.Add(carrotDetails);

            // Potato details
            var potatoDetails = new PlantDetails
            {
                PlantId = plants.First(p => p.Name == "Potato").Id,
                BloomSeason = BloomSeasonEnum.Summer,
                Lifecycle = PlantLifecycleEnum.Annual,
                WaterNeeds = WaterNeedsEnum.Medium,
                DifficultyLevel = DifficultyLevelEnum.Beginner,
                NativeTo = "South America (Andes region)",
                IdealPhLevel = "5.8-6.5",
                GrowingZones = "3-10",
                HeightAtMaturity = "45-60 cm",
                SpreadAtMaturity = "30-45 cm",
                DaysToGermination = "14-21 days (for sprouting seed potatoes)",
                DaysToMaturity = "70-120 days (depending on variety)",
                PlantingDepth = "10-15 cm",
                SpacingBetweenPlants = "30-38 cm",
                Purposes = new List<PlantPurposeEnum> { PlantPurposeEnum.Edible },
                PropagationMethods = "Seed potatoes (tuber sections with eyes)",
                PruningInstructions = "No pruning necessary. Some gardeners remove flowers to direct energy to tuber production.",
                PestManagement = "Watch for Colorado potato beetles, aphids, and wireworms. Use row covers early in season and practice crop rotation.",
                DiseaseManagement = "Susceptible to blight, scab, and various rots. Use certified disease-free seed potatoes and practice crop rotation.",
                FertilizationSchedule = "Apply balanced fertilizer at planting time. Side-dress when plants are about 15 cm tall.",
                WinterCare = "Annual in most regions. Store harvested tubers in cool, dark conditions.",
                HarvestingTips = "For new potatoes, harvest when plants begin to flower. For mature potatoes, harvest after vines die back.",
                StorageTips = "Cure for 1-2 weeks in a dark place at 15-20°C with high humidity. Store at 4-10°C in dark, well-ventilated conditions.",
                CulinaryUses = "Boiled, baked, fried, mashed. Essential in many Romanian dishes including mămăligă cu cartofi (polenta with potatoes).",
                MedicinalUses = "Raw potato juice has been used topically for skin conditions. Potatoes contain resistant starch beneficial for gut health.",
                HistoricalNotes = "Introduced to Europe in the 16th century. Initially met with suspicion but became a staple crop by the 18th century.",
                AdditionalNotes = "Romania has a strong tradition of potato cultivation, especially in the Transylvania region.",
                ImageUrls = new List<string> {
                    "https://kellogggarden.com/wp-content/uploads/2020/03/Potato-Roots.jpg",
                    "https://www.rhs.org.uk/getmedia/0d8f8c9c-b6b1-459b-b459-4172a167cc1b/potato-harvest-webuse-PUB0027730.jpg?width=940&height=624&ext=.jpg",
                    "https://cropaia.com/wp-content/uploads/Potato-field.jpg"
                },
                CompanionPlantIds = new List<int> { plants.First(p => p.Name == "Carrot").Id }
            };
            plantDetails.Add(potatoDetails);

            context.PlantDetails.AddRange(plantDetails);
        }
    }
}
