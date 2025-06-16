namespace StudioScheduler.PlaywrightTests.ApiTests;

[TestFixture]
public class AttendanceApiTests : BaseApiTest
{
    // Using a known schedule ID from the seeded data
    private const string ValidScheduleId = "c1a2b3c4-1234-5678-9abc-def012345614";
    private const string InvalidScheduleId = "invalid-guid";
    private const string NonExistentScheduleId = "00000000-0000-0000-0000-000000000000";

    [Test]
    public async Task GetClassSchedule_WithValidId_ShouldReturnSuccessWithValidStructure()
    {
        // Act
        var response = await ApiContext.GetAsync($"/api/attendance/class/{ValidScheduleId}");
        
        // Assert
        await AssertSuccessfulResponse(response);
        
        var classAttendance = await DeserializeResponse<ClassAttendanceDto>(response);
        
        Assert.That(classAttendance, Is.Not.Null, "Class attendance should not be null");
        Assert.That(classAttendance.ScheduleId, Is.Not.Null.And.Not.Empty, "Schedule ID should not be empty");
        Assert.That(classAttendance.ClassName, Is.Not.Null.And.Not.Empty, "Class name should not be empty");
        Assert.That(classAttendance.DayOfWeek, Is.Not.Null.And.Not.Empty, "Day of week should not be empty");
        Assert.That(classAttendance.Level, Is.Not.Null.And.Not.Empty, "Level should not be empty");
        Assert.That(classAttendance.Style, Is.Not.Null.And.Not.Empty, "Style should not be empty");
        Assert.That(classAttendance.EnrolledStudents, Is.Not.Null, "Enrolled students list should not be null");
    }

    [Test]
    public async Task GetClassSchedule_WithValidId_ShouldReturnCorrectScheduleId()
    {
        // Act
        var response = await ApiContext.GetAsync($"/api/attendance/class/{ValidScheduleId}");
        await AssertSuccessfulResponse(response);
        
        var classAttendance = await DeserializeResponse<ClassAttendanceDto>(response);
        
        // Assert
        Assert.That(classAttendance?.ScheduleId.ToLower(), Is.EqualTo(ValidScheduleId.ToLower()), 
            "Returned schedule ID should match requested ID");
    }

    [Test]
    public async Task GetClassSchedule_WithValidId_ShouldReturnValidStartTime()
    {
        // Act
        var response = await ApiContext.GetAsync($"/api/attendance/class/{ValidScheduleId}");
        await AssertSuccessfulResponse(response);
        
        var classAttendance = await DeserializeResponse<ClassAttendanceDto>(response);
        
        // Assert
        Assert.That(classAttendance?.StartTime, Is.Not.EqualTo(default(DateTime)), 
            "Start time should be a valid date/time");
        
        // Verify it's a reasonable time (dance classes are typically in the evening)
        var startTime = classAttendance!.StartTime;
        Assert.That(startTime.Hour, Is.GreaterThanOrEqualTo(16).And.LessThanOrEqualTo(23), 
            "Dance classes should typically be between 4 PM and 11 PM");
    }

    [Test]
    public async Task GetClassSchedule_WithValidId_ShouldReturnValidDayOfWeek()
    {
        // Act
        var response = await ApiContext.GetAsync($"/api/attendance/class/{ValidScheduleId}");
        await AssertSuccessfulResponse(response);
        
        var classAttendance = await DeserializeResponse<ClassAttendanceDto>(response);
        
        // Assert
        var validDays = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
        Assert.That(validDays, Contains.Item(classAttendance?.DayOfWeek), 
            "Day of week should be a valid day name");
    }

    [Test]
    public async Task GetClassSchedule_WithValidId_ShouldReturnValidEnrolledStudents()
    {
        // Act
        var response = await ApiContext.GetAsync($"/api/attendance/class/{ValidScheduleId}");
        await AssertSuccessfulResponse(response);
        
        var classAttendance = await DeserializeResponse<ClassAttendanceDto>(response);
        
        // Assert
        Assert.That(classAttendance?.EnrolledStudents, Is.Not.Null);
        
        // If there are enrolled students, validate their structure
        if (classAttendance!.EnrolledStudents.Any())
        {
            var firstStudent = classAttendance.EnrolledStudents.First();
            
            Assert.That(firstStudent.StudentId, Is.Not.Null.And.Not.Empty, "Student ID should not be empty");
            Assert.That(firstStudent.FirstName, Is.Not.Null.And.Not.Empty, "First name should not be empty");
            Assert.That(firstStudent.LastName, Is.Not.Null.And.Not.Empty, "Last name should not be empty");
            Assert.That(firstStudent.AttendanceHistory, Is.Not.Null, "Attendance history should not be null");
            
            // Validate attendance history structure if present
            if (firstStudent.AttendanceHistory.Any())
            {
                var firstAttendance = firstStudent.AttendanceHistory.First();
                Assert.That(firstAttendance.ClassDate, Is.Not.EqualTo(default(DateTime)), 
                    "Class date should be a valid date");
            }
        }
    }

    [Test]
    public async Task GetClassSchedule_WithValidId_ShouldReturnCorrectDanceStyleAndLevel()
    {
        // Act
        var response = await ApiContext.GetAsync($"/api/attendance/class/{ValidScheduleId}");
        await AssertSuccessfulResponse(response);
        
        var classAttendance = await DeserializeResponse<ClassAttendanceDto>(response);
        
        // Assert
        Assert.That(classAttendance?.Style, Is.Not.Null.And.Not.Empty);
        Assert.That(classAttendance?.Level, Is.Not.Null.And.Not.Empty);
        
        // Verify it's a known dance style from SalsaMe studio
        var knownStyles = new[] { "SALSA", "BACHATA", "KIZOMBA", "ZOUK", "RUEDA", "HIGH HEELS", "LADIES STYLING" };
        var styleMatches = knownStyles.Any(style => 
            classAttendance!.Style.ToUpper().Contains(style) || 
            classAttendance.ClassName.ToUpper().Contains(style));
        
        Assert.That(styleMatches, Is.True, 
            $"Style should match known dance styles. Found: {classAttendance!.Style}");
        
        // Verify level format
        Assert.That(classAttendance.Level, Does.Match(@"^(Level\s+)?(P[1-9]|S[1-9]|Z|OPEN(\s+level)?)"), 
            "Level should be in format like 'Level P1', 'Level S3', 'OPEN level', etc.");
    }

    [Test]
    public async Task GetClassSchedule_WithInvalidGuid_ShouldReturnBadRequest()
    {
        // Act
        var response = await ApiContext.GetAsync($"/api/attendance/class/{InvalidScheduleId}");
        
        // Assert
        await AssertErrorResponse(response, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetClassSchedule_WithNonExistentId_ShouldReturnNotFound()
    {
        // Act
        var response = await ApiContext.GetAsync($"/api/attendance/class/{NonExistentScheduleId}");
        
        // Assert
        await AssertErrorResponse(response, HttpStatusCode.NotFound);
    }

    [Test]
    public async Task GetClassSchedule_WithValidId_ShouldHaveCorrectResponseHeaders()
    {
        // Act
        var response = await ApiContext.GetAsync($"/api/attendance/class/{ValidScheduleId}");
        
        // Assert
        await AssertSuccessfulResponse(response);
        
        var headers = response.Headers;
        
        // Verify content type
        Assert.That(headers.ContainsKey("content-type"), Is.True, "Response should have content-type header");
        Assert.That(headers["content-type"], Does.Contain("application/json"), 
            "Content type should be JSON");
    }

    [Test]
    public async Task GetClassSchedule_WithValidId_ShouldReturnValidJson()
    {
        // Act
        var response = await ApiContext.GetAsync($"/api/attendance/class/{ValidScheduleId}");
        await AssertSuccessfulResponse(response);
        
        var jsonContent = await response.TextAsync();
        
        // Assert
        Assert.That(jsonContent, Is.Not.Null.And.Not.Empty, "Response should contain JSON content");
        
        // Verify it's valid JSON by attempting to parse
        Assert.DoesNotThrow(() => JsonSerializer.Deserialize<object>(jsonContent), 
            "Response should be valid JSON");
    }

    [Test]
    public async Task GetClassSchedule_WithValidId_ShouldReturnConsistentData()
    {
        // Act - Make the same request twice
        var response1 = await ApiContext.GetAsync($"/api/attendance/class/{ValidScheduleId}");
        var response2 = await ApiContext.GetAsync($"/api/attendance/class/{ValidScheduleId}");
        
        // Assert
        await AssertSuccessfulResponse(response1);
        await AssertSuccessfulResponse(response2);
        
        var classAttendance1 = await DeserializeResponse<ClassAttendanceDto>(response1);
        var classAttendance2 = await DeserializeResponse<ClassAttendanceDto>(response2);
        
        // Verify consistent data
        Assert.That(classAttendance1?.ScheduleId, Is.EqualTo(classAttendance2?.ScheduleId));
        Assert.That(classAttendance1?.ClassName, Is.EqualTo(classAttendance2?.ClassName));
        Assert.That(classAttendance1?.StartTime, Is.EqualTo(classAttendance2?.StartTime));
        Assert.That(classAttendance1?.DayOfWeek, Is.EqualTo(classAttendance2?.DayOfWeek));
        Assert.That(classAttendance1?.Level, Is.EqualTo(classAttendance2?.Level));
        Assert.That(classAttendance1?.Style, Is.EqualTo(classAttendance2?.Style));
    }

    [Test]
    public async Task GetClassSchedule_ResponseTime_ShouldBeReasonable()
    {
        // Arrange
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        // Act
        var response = await ApiContext.GetAsync($"/api/attendance/class/{ValidScheduleId}");
        stopwatch.Stop();
        
        // Assert
        await AssertSuccessfulResponse(response);
        
        // Response should be within reasonable time (less than 2 seconds for API call)
        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(2000), 
            $"API response should be fast. Took {stopwatch.ElapsedMilliseconds}ms");
    }
}
