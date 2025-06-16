using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using StudioScheduler.Core.Validators;
using StudioScheduler.Infrastructure;
using StudioScheduler.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add OpenAPI/Swagger
builder.Services.AddOpenApi();

// Add Entity Framework with SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add repositories and services
builder.Services.AddEfRepositories(); // Using Entity Framework repositories with SQLite

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<DanceClassValidator>();

// Add CORS
var allowedOrigin = "http://localhost:5069";
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClient",
        policy => policy.WithOrigins(allowedOrigin)
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowClient");

// Map controllers
app.MapControllers();

// Seed the database with initial data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var seedingService = scope.ServiceProvider.GetRequiredService<StudioScheduler.Infrastructure.Services.DataSeedingService>();
    
    // Ensure database is created and run migrations
    await context.Database.EnsureCreatedAsync();
    
    // Seed data from JSON files
    await seedingService.SeedDataAsync();
}

app.Run();

// Make the implicit Program class public so test projects can access it
public partial class Program { }
