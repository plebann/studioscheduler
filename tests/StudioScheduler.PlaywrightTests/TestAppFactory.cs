using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using StudioScheduler.Core.Enums;
using StudioScheduler.Core.Models;
using StudioScheduler.Core.Validators;
using StudioScheduler.Infrastructure;
using StudioScheduler.Infrastructure.Data;
using StudioScheduler.Infrastructure.Services;

namespace StudioScheduler.PlaywrightTests;

public static class TestAppFactory
{
    public static WebApplication CreateTestApp(string testDatabasePath)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            // Use test environment
            EnvironmentName = "Test",
            // Configure for testing
            Args = Array.Empty<string>()
        });

        // Configure test-specific settings
        builder.Configuration.AddInMemoryCollection(new[]
        {
            new KeyValuePair<string, string?>("ConnectionStrings:DefaultConnection", 
                $"Data Source={testDatabasePath}"),
            new KeyValuePair<string, string?>("Logging:LogLevel:Default", "Warning"),
            new KeyValuePair<string, string?>("Environment", "Test")
        });

        // Add services to the container (same as main app)
        // Explicitly reference the Server assembly to ensure controller discovery
        builder.Services.AddControllers()
            .AddApplicationPart(typeof(StudioScheduler.Server.Controllers.SchedulesController).Assembly);

        // Add OpenAPI/Swagger (optional for tests, but helps with debugging)
        builder.Services.AddOpenApi();

        // Add Entity Framework with test SQLite database
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite($"Data Source={testDatabasePath}"));

        // Add repositories and services
        builder.Services.AddEfRepositories();

        // Add FluentValidation
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddValidatorsFromAssemblyContaining<DanceClassValidator>();

        // Add CORS (relaxed for testing)
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                policy => policy.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod());
        });

        // Configure Kestrel for testing with dynamic port
        builder.WebHost.UseUrls("http://127.0.0.1:0"); // Use loopback with dynamic port

        var app = builder.Build();

        // Configure the HTTP request pipeline for testing
        // Note: No HTTPS redirection in tests to avoid complications
        
        // Enable CORS
        app.UseCors("AllowAll");

        // Map controllers
        app.MapControllers();

        // Add development endpoints for debugging (test environment only)
        if (app.Environment.EnvironmentName == "Test")
        {
            app.MapOpenApi();
            
            // Add a simple test endpoint to verify the server is working
            app.MapGet("/test", () => "Test server is running!");
            
            // Add endpoint to list all registered routes (for debugging)
            app.MapGet("/debug/routes", (IServiceProvider services) =>
            {
                var endpointDataSource = services.GetService<EndpointDataSource>();
                if (endpointDataSource != null)
                {
                    var endpoints = endpointDataSource.Endpoints
                        .OfType<RouteEndpoint>()
                        .Select(e => new { 
                            Pattern = e.RoutePattern.RawText,
                            HttpMethods = e.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault()?.HttpMethods
                        });
                    return Results.Json(endpoints);
                }
                return Results.Json(new { message = "No endpoints found" });
            });
        }

        return app;
    }

    public static async Task<ApplicationDbContext> SetupTestDatabaseAsync(WebApplication app, string testDatabasePath)
    {
        // Copy existing database to test database path
        var mainDatabasePath = Path.Combine(
            Directory.GetCurrentDirectory(), 
            "..", "..", "..", "..", 
            "src", "StudioScheduler.Server", "studioscheduler.db");
        
        if (File.Exists(mainDatabasePath))
        {
            // Copy the main database to test location
            File.Copy(mainDatabasePath, testDatabasePath, true);
            Console.WriteLine($"DEBUG: Copied database from {mainDatabasePath} to {testDatabasePath}");
        }
        else
        {
            Console.WriteLine($"DEBUG: Main database not found at {mainDatabasePath}");
            // If main database doesn't exist, create empty database and try seeding
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.EnsureCreatedAsync();
            
            try
            {
                var seedingService = scope.ServiceProvider.GetRequiredService<DataSeedingService>();
                await seedingService.SeedDataAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG: Seeding failed: {ex.Message}");
            }
        }

        // Return context for verification
        using var verifyScope = app.Services.CreateScope();
        var verifyContext = verifyScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var scheduleCount = await verifyContext.Schedules.CountAsync();
        Console.WriteLine($"DEBUG: Database setup complete. Schedule count: {scheduleCount}");
        
        // Debug: Show first few schedule IDs to help with tests
        var scheduleIds = await verifyContext.Schedules
            .Take(5)
            .Select(s => s.Id.ToString())
            .ToListAsync();
        Console.WriteLine($"DEBUG: First 5 schedule IDs: {string.Join(", ", scheduleIds)}");
        
        // Check if the test ID exists (with different case)
        var testId = "c1a2b3c4-1234-5678-9abc-def012345614";
        var existsLower = await verifyContext.Schedules.AnyAsync(s => s.Id.ToString().ToLower() == testId);
        var existsUpper = await verifyContext.Schedules.AnyAsync(s => s.Id.ToString().ToUpper() == testId.ToUpper());
        Console.WriteLine($"DEBUG: Test ID exists (lower): {existsLower}, exists (upper): {existsUpper}");
        
        return verifyContext;
    }
}
