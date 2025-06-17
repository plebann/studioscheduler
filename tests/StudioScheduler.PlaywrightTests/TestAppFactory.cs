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
using System.Text.Json;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Net;
using StudioScheduler.Shared.Dtos;

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
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();
        
        // Create test data programmatically
        await SeedTestDataAsync(context);
        
        // Debug: Show created schedule IDs
        var scheduleIds = await context.Schedules
            .Take(5)
            .Select(s => s.Id.ToString())
            .ToListAsync();
        Console.WriteLine($"DEBUG: Created {await context.Schedules.CountAsync()} schedules with IDs: {string.Join(", ", scheduleIds)}");
        
        return context;
    }

    private static async Task SeedTestDataAsync(ApplicationDbContext context)
    {
        // Create test location
        var testLocation = new Location
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "Test Studio",
            Address = "123 Test Street, Test City, 12345",
            Description = "Test dance studio for integration tests",
            Capacity = 100,
            OpeningTime = TimeSpan.FromHours(10), // 10 AM
            ClosingTime = TimeSpan.FromHours(23), // 11 PM
            IsActive = true
        };
        context.Locations.Add(testLocation);

        // Create test dance classes
        var salsaClass = new DanceClass
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Name = "Salsa Beginners",
            Description = "Beginner level salsa class",
            Style = "SALSA",
            IsActive = true
        };
        
        var bachataClass = new DanceClass
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Name = "Bachata Intermediate",
            Description = "Intermediate level bachata class",
            Style = "BACHATA",
            IsActive = true
        };
        
        context.DanceClasses.AddRange(salsaClass, bachataClass);

        // Create test schedules for specific weekdays
        var now = DateTime.UtcNow;
        var mondaySchedule = new Schedule
        {
            Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            Name = "Monday Salsa",
            LocationId = testLocation.Id,
            DanceClassId = salsaClass.Id,
            StartTime = GetNextWeekday(DayOfWeek.Monday).AddHours(19), // Next Monday 7 PM
            Duration = TimeSpan.FromHours(1),
            EffectiveFrom = DateTime.UtcNow.AddDays(-30),
            IsActive = true,
            IsCancelled = false,
            Level = "Level P1",
            Capacity = 20
        };

        var tuesdaySchedule = new Schedule
        {
            Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
            Name = "Tuesday Bachata",
            LocationId = testLocation.Id,
            DanceClassId = bachataClass.Id,
            StartTime = GetNextWeekday(DayOfWeek.Tuesday).AddHours(20), // Next Tuesday 8 PM
            Duration = TimeSpan.FromHours(1),
            EffectiveFrom = DateTime.UtcNow.AddDays(-30),
            IsActive = true,
            IsCancelled = false,
            Level = "Level S2", 
            Capacity = 15
        };

        var wednesdaySchedule = new Schedule
        {
            Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
            Name = "Wednesday Kizomba",
            LocationId = testLocation.Id,
            DanceClassId = salsaClass.Id, // Reuse salsa class but different schedule
            StartTime = GetNextWeekday(DayOfWeek.Wednesday).AddHours(18), // Next Wednesday 6 PM
            Duration = TimeSpan.FromMinutes(90),
            EffectiveFrom = DateTime.UtcNow.AddDays(-30),
            IsActive = true,
            IsCancelled = false,
            Level = "Level P1",
            Capacity = 18
        };

        context.Schedules.AddRange(mondaySchedule, tuesdaySchedule, wednesdaySchedule);

        // Create test students
        var testStudent = new Student
        {
            Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
            FirstName = "Test",
            LastName = "Student",
            Email = "test.student@example.com",
            PasswordHash = "test_hash",
            Phone = "+1234567890",
            IsActive = true,
            Role = UserRole.Student
        };
        
        context.Students.Add(testStudent);

        // Create test enrollments
        var enrollment = new Enrollment
        {
            Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
            StudentId = testStudent.Id,
            ScheduleId = mondaySchedule.Id,
            EnrolledDate = DateTime.UtcNow.AddDays(-7),
            IsActive = true
        };
        
        context.Enrollments.Add(enrollment);

        await context.SaveChangesAsync();
    }

    private static DateTime GetNextWeekday(DayOfWeek targetDay)
    {
        var today = DateTime.UtcNow.Date;
        var daysUntilTarget = ((int)targetDay - (int)today.DayOfWeek + 7) % 7;
        if (daysUntilTarget == 0) daysUntilTarget = 7; // If today is the target day, get next week's
        return today.AddDays(daysUntilTarget);
    }
}
