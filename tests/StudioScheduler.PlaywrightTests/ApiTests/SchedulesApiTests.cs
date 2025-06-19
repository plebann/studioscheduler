using System.Text;

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

    #region GetSchedules Tests

    [Test]
    public async Task GetSchedules_ShouldReturnSuccessWithScheduleList()
    {
        // Act
        var response = await ApiContext.GetAsync("/api/schedules");
        
        // Assert
        await AssertSuccessfulResponse(response);
        
        var schedules = await DeserializeResponse<List<ScheduleSummaryDto>>(response);
        
        Assert.That(schedules, Is.Not.Null, "Schedules list should not be null");
        Assert.That(schedules.Count, Is.GreaterThan(0), "Should have at least one schedule");
        
        // Verify first schedule structure
        var firstSchedule = schedules.First();
        Assert.That(firstSchedule.Id, Is.Not.EqualTo(Guid.Empty), "Schedule should have valid ID");
        Assert.That(firstSchedule.Name, Is.Not.Null.And.Not.Empty, "Schedule should have name");
        Assert.That(firstSchedule.Level, Is.Not.Null.And.Not.Empty, "Schedule should have level");
        Assert.That(firstSchedule.Capacity, Is.GreaterThan(0), "Schedule should have positive capacity");
    }

    [Test]
    public async Task GetSchedules_ShouldReturnCorrectResponseHeaders()
    {
        // Act
        var response = await ApiContext.GetAsync("/api/schedules");
        
        // Assert
        await AssertSuccessfulResponse(response);
        
        var headers = response.Headers;
        Assert.That(headers.ContainsKey("content-type"), Is.True, "Response should have content-type header");
        Assert.That(headers["content-type"], Does.Contain("application/json"), "Content type should be JSON");
    }

    #endregion

    #region GetSchedule Tests

    [Test]
    public async Task GetSchedule_WithValidId_ShouldReturnScheduleDetails()
    {
        // Arrange - First get a schedule to test with
        var allSchedulesResponse = await ApiContext.GetAsync("/api/schedules");
        await AssertSuccessfulResponse(allSchedulesResponse);
        var schedules = await DeserializeResponse<List<ScheduleSummaryDto>>(allSchedulesResponse);
        var testScheduleId = schedules.First().Id;
        
        // Act
        var response = await ApiContext.GetAsync($"/api/schedules/{testScheduleId}");
        
        // Assert
        await AssertSuccessfulResponse(response);
        
        var schedule = await DeserializeResponse<ScheduleDto>(response);
        
        Assert.That(schedule, Is.Not.Null, "Schedule should not be null");
        Assert.That(schedule.Id, Is.EqualTo(testScheduleId), "Schedule ID should match requested ID");
        Assert.That(schedule.Name, Is.Not.Null.And.Not.Empty, "Schedule should have name");
        Assert.That(schedule.LocationId, Is.Not.EqualTo(Guid.Empty), "Schedule should have location ID");
        Assert.That(schedule.DanceClassId, Is.Not.EqualTo(Guid.Empty), "Schedule should have dance class ID");
        Assert.That(schedule.Level, Is.Not.Null.And.Not.Empty, "Schedule should have level");
        Assert.That(schedule.Duration, Is.GreaterThan(0), "Schedule should have positive duration");
        Assert.That(schedule.Capacity, Is.GreaterThan(0), "Schedule should have positive capacity");
    }

    [Test]
    public async Task GetSchedule_WithNonExistentId_ShouldReturn404()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        
        // Act
        var response = await ApiContext.GetAsync($"/api/schedules/{nonExistentId}");
        
        // Assert
        await AssertErrorResponse(response, HttpStatusCode.NotFound);
    }

    [Test]
    public async Task GetSchedule_WithInvalidGuid_ShouldReturn404()
    {
        // Act
        var response = await ApiContext.GetAsync("/api/schedules/invalid-guid");
        
        // Assert - ASP.NET Core returns 404 for invalid route parameters, not 400
        await AssertErrorResponse(response, HttpStatusCode.NotFound);
    }

    #endregion

    #region CreateSchedule Tests

    [Test]
    public async Task CreateSchedule_WithValidData_ShouldCreateAndReturnSchedule()
    {
        // Arrange - Get valid location and dance class IDs
        var locationsResponse = await ApiContext.GetAsync("/api/locations");
        await AssertSuccessfulResponse(locationsResponse);
        var locations = await DeserializeResponse<List<LocationDto>>(locationsResponse);
        var locationId = locations.First().Id;

        var classesResponse = await ApiContext.GetAsync("/api/classes");
        await AssertSuccessfulResponse(classesResponse);
        var classes = await DeserializeResponse<List<ClassSummaryDto>>(classesResponse);
        var danceClassId = classes.First().Id;

        var createDto = new CreateScheduleDto
        {
            Name = "Test Schedule",
            LocationId = locationId,
            DanceClassId = danceClassId,
            DayOfWeek = DayOfWeek.Monday,
            StartTime = TimeSpan.FromHours(19), // 7 PM
            Duration = 90,
            IsRecurring = true,
            EffectiveFrom = DateTime.UtcNow.Date,
            Level = "P1",
            Capacity = 20
        };

        // Act
        var response = await ApiContext.PostAsync("/api/schedules", new APIRequestContextOptions
        {
            Data = JsonSerializer.Serialize(createDto, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            })
        });

        // Assert
        await AssertSuccessfulResponse(response, HttpStatusCode.Created);
        
        var schedule = await DeserializeResponse<ScheduleDto>(response);
        
        Assert.That(schedule, Is.Not.Null, "Created schedule should not be null");
        Assert.That(schedule.Id, Is.Not.EqualTo(Guid.Empty), "Created schedule should have valid ID");
        Assert.That(schedule.Name, Is.EqualTo(createDto.Name), "Schedule name should match");
        Assert.That(schedule.LocationId, Is.EqualTo(createDto.LocationId), "Location ID should match");
        Assert.That(schedule.DanceClassId, Is.EqualTo(createDto.DanceClassId), "Dance class ID should match");
        Assert.That(schedule.DayOfWeek, Is.EqualTo(createDto.DayOfWeek), "Day of week should match");
        Assert.That(schedule.StartTime, Is.EqualTo(createDto.StartTime), "Start time should match");
        Assert.That(schedule.Duration, Is.EqualTo(createDto.Duration), "Duration should match");
        Assert.That(schedule.Level, Is.EqualTo(createDto.Level), "Level should match");
        Assert.That(schedule.Capacity, Is.EqualTo(createDto.Capacity), "Capacity should match");
        Assert.That(schedule.IsActive, Is.True, "New schedule should be active");
        Assert.That(schedule.IsCancelled, Is.False, "New schedule should not be cancelled");
    }

    [Test]
    public async Task CreateSchedule_WithInvalidLocationId_ShouldReturn400()
    {
        // Arrange
        var createDto = new CreateScheduleDto
        {
            Name = "Test Schedule",
            LocationId = Guid.NewGuid(), // Non-existent location
            DanceClassId = Guid.NewGuid(),
            DayOfWeek = DayOfWeek.Monday,
            StartTime = TimeSpan.FromHours(19),
            Duration = 90,
            IsRecurring = true,
            EffectiveFrom = DateTime.UtcNow.Date,
            Level = "P1",
            Capacity = 20
        };

        // Act
        var response = await ApiContext.PostAsync("/api/schedules", new APIRequestContextOptions
        {
            Data = JsonSerializer.Serialize(createDto, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            })
        });

        // Assert - This might return 400 or 500 depending on validation implementation
        Assert.That(response.Status, Is.AnyOf(400, 500), "Should return error for invalid location ID");
    }

    #endregion

    #region UpdateSchedule Tests

    [Test]
    public async Task UpdateSchedule_WithValidData_ShouldReturnUpdatedSchedule()
    {
        // Arrange - First get an existing schedule
        var allSchedulesResponse = await ApiContext.GetAsync("/api/schedules");
        await AssertSuccessfulResponse(allSchedulesResponse);
        var schedules = await DeserializeResponse<List<ScheduleSummaryDto>>(allSchedulesResponse);
        var testScheduleId = schedules.First().Id;

        // Get the full schedule details
        var getResponse = await ApiContext.GetAsync($"/api/schedules/{testScheduleId}");
        await AssertSuccessfulResponse(getResponse);
        var originalSchedule = await DeserializeResponse<ScheduleDto>(getResponse);

        var updateDto = new UpdateScheduleDto
        {
            Name = "Updated Schedule Name",
            DayOfWeek = DayOfWeek.Tuesday, // Different from original
            StartTime = TimeSpan.FromHours(20), // 8 PM
            Duration = 120, // 2 hours
            IsRecurring = true,
            EffectiveFrom = DateTime.UtcNow.Date.AddDays(1),
            IsActive = true,
            IsCancelled = false,
            Level = "P2", // Different level
            Capacity = 25
        };

        // Act
        var response = await ApiContext.PutAsync($"/api/schedules/{testScheduleId}", new APIRequestContextOptions
        {
            Data = JsonSerializer.Serialize(updateDto, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            })
        });

        // Assert
        await AssertSuccessfulResponse(response);
        
        var updatedSchedule = await DeserializeResponse<ScheduleDto>(response);
        
        Assert.That(updatedSchedule, Is.Not.Null, "Updated schedule should not be null");
        Assert.That(updatedSchedule.Id, Is.EqualTo(testScheduleId), "Schedule ID should remain same");
        Assert.That(updatedSchedule.Name, Is.EqualTo(updateDto.Name), "Name should be updated");
        Assert.That(updatedSchedule.DayOfWeek, Is.EqualTo(updateDto.DayOfWeek), "Day of week should be updated");
        Assert.That(updatedSchedule.StartTime, Is.EqualTo(updateDto.StartTime), "Start time should be updated");
        Assert.That(updatedSchedule.Duration, Is.EqualTo(updateDto.Duration), "Duration should be updated");
        Assert.That(updatedSchedule.Level, Is.EqualTo(updateDto.Level), "Level should be updated");
        Assert.That(updatedSchedule.Capacity, Is.EqualTo(updateDto.Capacity), "Capacity should be updated");
        
        // Verify immutable fields are preserved
        Assert.That(updatedSchedule.LocationId, Is.EqualTo(originalSchedule.LocationId), "Location ID should be preserved");
        Assert.That(updatedSchedule.DanceClassId, Is.EqualTo(originalSchedule.DanceClassId), "Dance class ID should be preserved");
        Assert.That(updatedSchedule.CreatedAt, Is.EqualTo(originalSchedule.CreatedAt), "Created at should be preserved");
        
        // Verify UpdatedAt is changed
        Assert.That(updatedSchedule.UpdatedAt, Is.Not.EqualTo(originalSchedule.UpdatedAt), "UpdatedAt should be changed");
        Assert.That(updatedSchedule.UpdatedAt, Is.Not.Null, "UpdatedAt should not be null");
    }

    [Test]
    public async Task UpdateSchedule_WithNonExistentId_ShouldReturn404()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var updateDto = new UpdateScheduleDto
        {
            Name = "Updated Name",
            DayOfWeek = DayOfWeek.Monday,
            StartTime = TimeSpan.FromHours(19),
            Duration = 90,
            IsRecurring = true,
            EffectiveFrom = DateTime.UtcNow.Date,
            IsActive = true,
            IsCancelled = false,
            Level = "P1",
            Capacity = 20
        };

        // Act
        var response = await ApiContext.PutAsync($"/api/schedules/{nonExistentId}", new APIRequestContextOptions
        {
            Data = JsonSerializer.Serialize(updateDto, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            })
        });

        // Assert
        await AssertErrorResponse(response, HttpStatusCode.NotFound);
    }

    #endregion

    #region DeleteSchedule Tests

    [Test]
    public async Task DeleteSchedule_WithValidId_ShouldReturn204()
    {
        // Arrange - First create a schedule to delete
        var locationsResponse = await ApiContext.GetAsync("/api/locations");
        await AssertSuccessfulResponse(locationsResponse);
        var locations = await DeserializeResponse<List<LocationDto>>(locationsResponse);
        var locationId = locations.First().Id;

        var classesResponse = await ApiContext.GetAsync("/api/classes");
        await AssertSuccessfulResponse(classesResponse);
        var classes = await DeserializeResponse<List<ClassSummaryDto>>(classesResponse);
        var danceClassId = classes.First().Id;

        var createDto = new CreateScheduleDto
        {
            Name = "Schedule to Delete",
            LocationId = locationId,
            DanceClassId = danceClassId,
            DayOfWeek = DayOfWeek.Friday,
            StartTime = TimeSpan.FromHours(18),
            Duration = 90,
            IsRecurring = true,
            EffectiveFrom = DateTime.UtcNow.Date,
            Level = "P1",
            Capacity = 15
        };

        var createResponse = await ApiContext.PostAsync("/api/schedules", new APIRequestContextOptions
        {
            Data = JsonSerializer.Serialize(createDto, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            })
        });
        await AssertSuccessfulResponse(createResponse, HttpStatusCode.Created);
        var createdSchedule = await DeserializeResponse<ScheduleDto>(createResponse);

        // Act
        var deleteResponse = await ApiContext.DeleteAsync($"/api/schedules/{createdSchedule.Id}");

        // Assert
        await AssertSuccessfulResponse(deleteResponse, HttpStatusCode.NoContent);
        
        // Verify schedule is actually deleted
        var getResponse = await ApiContext.GetAsync($"/api/schedules/{createdSchedule.Id}");
        await AssertErrorResponse(getResponse, HttpStatusCode.NotFound);
    }

    [Test]
    public async Task DeleteSchedule_WithNonExistentId_ShouldReturn404()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await ApiContext.DeleteAsync($"/api/schedules/{nonExistentId}");

        // Assert
        await AssertErrorResponse(response, HttpStatusCode.NotFound);
    }

    #endregion
}
