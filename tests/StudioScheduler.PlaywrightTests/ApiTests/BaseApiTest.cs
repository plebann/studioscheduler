using Microsoft.AspNetCore.Builder;

namespace StudioScheduler.PlaywrightTests.ApiTests;

[TestFixture]
public abstract class BaseApiTest
{
    protected WebApplication TestApp { get; private set; } = null!;
    protected HttpClient HttpClient { get; private set; } = null!;
    protected IAPIRequestContext ApiContext { get; private set; } = null!;
    protected IPlaywright Playwright { get; private set; } = null!;
    
    private string _testDatabasePath = null!;
    private string _testAppBaseUrl = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        // Initialize Playwright
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        
        // Create unique test database path
        _testDatabasePath = Path.Combine(Path.GetTempPath(), $"studioscheduler-test-{Guid.NewGuid()}.db");
        
        // Create test app using modern .NET 9 pattern
        TestApp = TestAppFactory.CreateTestApp(_testDatabasePath);
        
        // Start the test app
        await TestApp.StartAsync();
        
        // Get the actual running URL (with dynamic port)
        _testAppBaseUrl = TestApp.Urls.First();
        Console.WriteLine($"DEBUG: Test app running at: {_testAppBaseUrl}");
        
        // Create HttpClient that points to the test app
        HttpClient = new HttpClient();
        HttpClient.BaseAddress = new Uri(_testAppBaseUrl);
        
        // Setup test database and seed data
        await TestAppFactory.SetupTestDatabaseAsync(TestApp, _testDatabasePath);
        
        // Create Playwright API context with the real test server URL
        ApiContext = await Playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = _testAppBaseUrl,
            IgnoreHTTPSErrors = true,
            ExtraHTTPHeaders = new Dictionary<string, string>
            {
                { "Accept", "application/json" },
                { "Content-Type", "application/json" }
            }
        });
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        // Dispose in reverse order
        if (ApiContext != null)
            await ApiContext.DisposeAsync();
        
        if (HttpClient != null)
            HttpClient.Dispose();
        
        if (TestApp != null)
        {
            await TestApp.StopAsync();
            await TestApp.DisposeAsync();
        }
        
        if (Playwright != null)
            Playwright.Dispose();
        
        // Cleanup test database with retry logic
        if (File.Exists(_testDatabasePath))
        {
            try
            {
                File.Delete(_testDatabasePath);
            }
            catch (IOException)
            {
                // Database might be locked, wait and try again
                await Task.Delay(100);
                try
                {
                    File.Delete(_testDatabasePath);
                }
                catch (IOException)
                {
                    // If still locked, just leave it for cleanup later
                    Console.WriteLine($"Warning: Could not delete test database {_testDatabasePath}");
                }
            }
        }
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
