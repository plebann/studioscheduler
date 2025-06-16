using Microsoft.EntityFrameworkCore;
using StudioScheduler.Infrastructure.Data;
using StudioScheduler.Infrastructure.Services;
using StudioScheduler.Server;

namespace StudioScheduler.PlaywrightTests.ApiTests;

[TestFixture]
public abstract class BaseApiTest
{
    protected WebApplicationFactory<Program> Factory { get; private set; } = null!;
    protected HttpClient HttpClient { get; private set; } = null!;
    protected IAPIRequestContext ApiContext { get; private set; } = null!;
    protected IPlaywright Playwright { get; private set; } = null!;
    
    private string _testDatabasePath = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        // Initialize Playwright
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        
        // Create unique test database path
        _testDatabasePath = Path.Combine(Path.GetTempPath(), $"studioscheduler-test-{Guid.NewGuid()}.db");
        
        // Create WebApplicationFactory with test configuration
        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.Test.json", optional: false);
                    config.AddInMemoryCollection(new[]
                    {
                        new KeyValuePair<string, string?>("ConnectionStrings:DefaultConnection", 
                            $"Data Source={_testDatabasePath}")
                    });
                });

                builder.ConfigureServices(services =>
                {
                    // Remove any existing DbContext registration
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    // Add test database context
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlite($"Data Source={_testDatabasePath}"));
                });
            });

        // Create HttpClient
        HttpClient = Factory.CreateClient();
        
        // Setup test database and seed data
        await SetupTestDatabase();
        
        // Create Playwright API context
        ApiContext = await Playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = HttpClient.BaseAddress?.ToString(),
            IgnoreHTTPSErrors = true
        });
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await ApiContext.DisposeAsync();
        HttpClient.Dispose();
        Factory.Dispose();
        Playwright.Dispose();
        
        // Cleanup test database
        if (File.Exists(_testDatabasePath))
            File.Delete(_testDatabasePath);
    }

    private async Task SetupTestDatabase()
    {
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var seedingService = scope.ServiceProvider.GetRequiredService<DataSeedingService>();
        
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();
        
        // Seed test data
        await seedingService.SeedDataAsync();
    }

    protected async Task<T?> DeserializeResponse<T>(IAPIResponse response)
    {
        var jsonContent = await response.TextAsync();
        return JsonSerializer.Deserialize<T>(jsonContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    protected async Task AssertSuccessfulResponse(IAPIResponse response, HttpStatusCode expectedStatus = HttpStatusCode.OK)
    {
        Assert.That(response.Status, Is.EqualTo((int)expectedStatus), 
            $"Expected {expectedStatus} but got {response.Status}. Response: {await response.TextAsync()}");
    }

    protected async Task AssertErrorResponse(IAPIResponse response, HttpStatusCode expectedStatus)
    {
        Assert.That(response.Status, Is.EqualTo((int)expectedStatus),
            $"Expected {expectedStatus} but got {response.Status}. Response: {await response.TextAsync()}");
    }
}
