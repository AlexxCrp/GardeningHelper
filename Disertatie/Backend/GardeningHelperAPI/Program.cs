using GardeningHelperAPI.Services;
using GardeningHelperDatabase;
using GardeningHelperDatabase.Entities.Identity;
using GardeningHelperDatabase.Mappings;
using GardeningHelperDatabase.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace GardeningHelperAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add logging
            builder.Logging.AddConsole();

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GardeningHelper", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });

            // Add DbContext
            builder.Services.AddDbContext<GardeningHelperDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("GardeningDbConnectionString"),
                options => options.EnableRetryOnFailure()));

            // Add Identity
            builder.Services.AddIdentity<User, Role>()
                .AddRoles<Role>()
                .AddEntityFrameworkStores<GardeningHelperDbContext>()
                .AddDefaultTokenProviders();

            // Configure Identity options
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });

            // Add AutoMapper
            builder.Services.AddAutoMapper(typeof(UserProfile).Assembly);
            builder.Services.AddAutoMapper(typeof(PlantProfile).Assembly);

            // Add custom services
            builder.Services.AddScoped<IdentityService>();
            builder.Services.AddScoped<PlantService>();

            // Configure authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer("AuthScheme", options =>
            {
                options.SaveToken = true;
                var secretKey = builder.Configuration.GetSection("JwtSettings:SecretKey").Get<string>() ?? "safhwegfdjgfde8943jgla4920ghd@JG)(h(W)h)S(hS@";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ValidateIssuer = false
                };
            });

            // Configure authorization
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("User", policy => policy.RequireRole("User", "Admin").RequireAuthenticatedUser().AddAuthenticationSchemes("AuthScheme").Build());
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin").RequireAuthenticatedUser().AddAuthenticationSchemes("AuthScheme").Build());
            });

            // Configure CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("_allowSpecificOrigins", builder =>
                {
                    builder
                        .WithOrigins("localhost:3000", "http://localhost:3000", "https://localhost:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Seed the database
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var dbContext = services.GetRequiredService<GardeningHelperDbContext>();
                    var roleManager = services.GetRequiredService<RoleManager<Role>>();

                    // Ensure the database is migrated
                    dbContext.Database.Migrate();

                    // Seed the database
                    await SeedData.SeedDatabase(dbContext, roleManager);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("_allowSpecificOrigins");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}