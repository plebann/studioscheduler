namespace StudioScheduler.PlaywrightTests.ApiTests;

[TestFixture]
public class SchedulesApiTests : BaseApiTest
{
    [Test]
    public async Task GetWeeklySchedule_ShouldReturnSuccessWithValidStructure()
    {
        // Act
        var response = await ApiContext.GetAsync("/api/schedules/weekly");
        
        // Assert
        await AssertSuccessfulResponse(response);
        
        var weeklySchedule = await DeserializeResponse<WeeklyScheduleDto>(response);
        
        Assert.That(weeklySchedule, Is.Not.Null, "Weekly schedule should not be null");
        Assert.That(weeklySchedule.Schedule, Is.Not.Null, "Schedule dictionary should not be null");
        
        // Verify all days of the week are present
        var expectedDays = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
        foreach (var day in expectedDays)
        {
            Assert.That(weeklySchedule.Schedule.ContainsKey(day), Is.True, $"Schedule should contain {day}");
        }
    }

    [Test]
    public async Task GetWeeklySchedule_ShouldReturnCorrectScheduleSlotStructure()
    {
        // Act
        var response = await ApiContext.GetAsync("/api/schedules/weekly");
        await AssertSuccessfulResponse(response);
        
        var weeklySchedule = await DeserializeResponse<WeeklyScheduleDto>(response);
        
        // Assert
        Assert.That(weeklySchedule?.Schedule, Is.Not.Null);
        
        // Find a day with classes (should be most days except Saturday based on your data)
        var dayWithClasses = weeklySchedule.Schedule.FirstOrDefault(kvp => kvp.Value.Any());
        Assert.That(dayWithClasses.Value, Is.Not.Null, "Should have at least one day with classes");
        
        var firstClass = dayWithClasses.Value.First();
        
        // Verify ScheduleSlotDto structure
        Assert.That(firstClass.Id, Is.Not.EqualTo(Guid.Empty), "Schedule slot should have valid ID");
        Assert.That(firstClass.TimeSlot, Is.Not.Null.And.Not.Empty, "Time slot should not be empty");
        Assert.That(firstClass.DanceName, Is.Not.Null.And.Not.Empty, "Dance name should not be empty");
        Assert.That(firstClass.Level, Is.Not.Null.And.Not.Empty, "Level should not be empty");
        Assert.That(firstClass.Style, Is.Not.Null.And.Not.Empty, "Style should not be empty");
        Assert.That(firstClass.BackgroundColor, Is.Not.Null.And.Not.Empty, "Background color should not be empty");
        Assert.That(firstClass.EffectiveFrom, Is.Not.Null.And.Not.Empty, "Effective from should not be empty");
        
        // Verify time slot format (HH:mm - HH:mm)
        Assert.That(firstClass.TimeSlot, Does.Match(@"^\d{2}:\d{2} - \d{2}:\d{2}$"), 
            "Time slot should be in format HH:mm - HH:mm");
    }

    [Test]
    public async Task GetWeeklySchedule_ShouldReturnCorrectDanceStyles()
    {
        // Act
        var response = await ApiContext.GetAsync("/api/schedules/weekly");
        await AssertSuccessfulResponse(response);
        
        var weeklySchedule = await DeserializeResponse<WeeklyScheduleDto>(response);
        
        // Assert
        Assert.That(weeklySchedule?.Schedule, Is.Not.Null);
        
        var allClasses = weeklySchedule.Schedule.Values.SelectMany(classes => classes).ToList();
        Assert.That(allClasses, Is.Not.Empty, "Should have classes in the schedule");
        
        // Verify we have expected dance styles from SalsaMe studio
        var expectedStyles = new[] { "SALSA", "BACHATA", "KIZOMBA", "ZOUK", "RUEDA", "HIGH HEELS" };
        var actualStyles = allClasses.Select(c => c.Style.ToUpper()).Distinct().ToList();
        
        // Should have at least some of the expected styles
        var foundStyles = expectedStyles.Where(style => 
            actualStyles.Any(actual => actual.Contains(style))).ToList();
        
        Assert.That(foundStyles, Is.Not.Empty, 
            $"Should contain some expected dance styles. Found: {string.Join(", ", actualStyles)}");
    }

    [Test]
    public async Task GetWeeklySchedule_ShouldReturnCorrectColorMapping()
    {
        // Act
        var response = await ApiContext.GetAsync("/api/schedules/weekly");
        await AssertSuccessfulResponse(response);
        
        var weeklySchedule = await DeserializeResponse<WeeklyScheduleDto>(response);
        
        // Assert
        Assert.That(weeklySchedule?.Schedule, Is.Not.Null);
        
        var allClasses = weeklySchedule.Schedule.Values.SelectMany(classes => classes).ToList();
        
        // Verify color mapping based on dance styles
        foreach (var danceClass in allClasses)
        {
            Assert.That(danceClass.BackgroundColor, Does.StartWith("#"), 
                "Background color should be a hex color code");
            
            Assert.That(danceClass.BackgroundColor.Length, Is.EqualTo(7), 
                "Background color should be in format #RRGGBB");
            
            // Verify specific color mappings for known styles
            if (danceClass.Style.ToUpper().Contains("HIGH HEELS") || danceClass.Style.ToUpper().Contains("LADIES STYLING"))
            {
                Assert.That(danceClass.BackgroundColor.ToUpper(), Is.EqualTo("#E40046"), 
                    "High heels/ladies styling should have pink color");
            }
            else if (danceClass.Style.ToUpper().Contains("BACHATA"))
            {
                Assert.That(danceClass.BackgroundColor.ToUpper(), Is.EqualTo("#166693"), 
                    "Bachata should have blue color");
            }
            else if (danceClass.Style.ToUpper().Contains("KIZOMBA"))
            {
                Assert.That(danceClass.BackgroundColor.ToUpper(), Is.EqualTo("#007C5A"), 
                    "Kizomba should have green color");
            }
        }
    }

    [Test]
    public async Task GetWeeklySchedule_ShouldReturnClassesInTimeOrder()
    {
        // Act
        var response = await ApiContext.GetAsync("/api/schedules/weekly");
        await AssertSuccessfulResponse(response);
        
        var weeklySchedule = await DeserializeResponse<WeeklyScheduleDto>(response);
        
        // Assert
        Assert.That(weeklySchedule?.Schedule, Is.Not.Null);
        
        // Check that classes within each day are ordered by time
        foreach (var day in weeklySchedule.Schedule)
        {
            if (day.Value.Count <= 1) continue; // Skip days with 0 or 1 class
            
            var times = day.Value.Select(c => 
            {
                var timePart = c.TimeSlot.Split(" - ")[0];
                return TimeSpan.Parse(timePart);
            }).ToList();
            
            var sortedTimes = times.OrderBy(t => t).ToList();
            
            Assert.That(times, Is.EqualTo(sortedTimes), 
                $"Classes on {day.Key} should be ordered by start time");
        }
    }

    [Test]
    public async Task GetWeeklySchedule_ShouldHaveCorrectResponseHeaders()
    {
        // Act
        var response = await ApiContext.GetAsync("/api/schedules/weekly");
        
        // Assert
        await AssertSuccessfulResponse(response);
        
        var headers = response.Headers;
        
        // Verify content type
        Assert.That(headers.ContainsKey("content-type"), Is.True, "Response should have content-type header");
        Assert.That(headers["content-type"], Does.Contain("application/json"), 
            "Content type should be JSON");
    }

    [Test]
    public async Task GetWeeklySchedule_ShouldReturnValidJson()
    {
        // Act
        var response = await ApiContext.GetAsync("/api/schedules/weekly");
        await AssertSuccessfulResponse(response);
        
        var jsonContent = await response.TextAsync();
        
        // Assert
        Assert.That(jsonContent, Is.Not.Null.And.Not.Empty, "Response should contain JSON content");
        
        // Verify it's valid JSON by attempting to parse
        Assert.DoesNotThrow(() => JsonSerializer.Deserialize<object>(jsonContent), 
            "Response should be valid JSON");
    }
}
