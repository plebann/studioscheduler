using System.Text;
using StudioScheduler.Core.Enums;

namespace StudioScheduler.PlaywrightTests.ApiTests;

[TestFixture]
public class StudentsApiTests : BaseApiTest
{
    #region GetStudents Tests

    [Test]
    public async Task GetStudents_ShouldReturnSuccessWithStudentList()
    {
        // Act
        var response = await ApiContext.GetAsync("/api/students");
        
        // Assert
        await AssertSuccessfulResponse(response);
        
        var students = await DeserializeResponse<List<StudentDto>>(response);
        
        Assert.That(students, Is.Not.Null, "Students list should not be null");
        Assert.That(students.Count, Is.GreaterThan(0), "Should have at least one student");
        
        // Verify first student structure
        var firstStudent = students.First();
        Assert.That(firstStudent.Id, Is.Not.EqualTo(Guid.Empty), "Student should have valid ID");
        Assert.That(firstStudent.Name, Is.Not.Null.And.Not.Empty, "Student should have name");
        Assert.That(firstStudent.Email, Is.Not.Null.And.Not.Empty, "Student should have email");
        Assert.That(firstStudent.JoinDate, Is.Not.EqualTo(default(DateTime)), "Student should have join date");
        Assert.That(firstStudent.Role, Is.EqualTo(UserRole.Student), "Student should have Student role");
        Assert.That(firstStudent.IsActive, Is.True, "Student should be active by default");
    }

    [Test]
    public async Task GetStudents_ShouldReturnCorrectStudentStructure()
    {
        // Act
        var response = await ApiContext.GetAsync("/api/students");
        await AssertSuccessfulResponse(response);
        
        var students = await DeserializeResponse<List<StudentDto>>(response);
        
        // Assert
        Assert.That(students, Is.Not.Null.And.Not.Empty);
        
        var student = students.First();
        
        // Verify StudentDto structure
        Assert.That(student.Id, Is.Not.EqualTo(Guid.Empty), "Student should have valid ID");
        Assert.That(student.Name, Is.Not.Null.And.Not.Empty, "Student should have name");
        Assert.That(student.Email, Is.Not.Null.And.Not.Empty, "Student should have email");
        Assert.That(student.Email, Does.Contain("@"), "Email should be valid format");
        Assert.That(student.JoinDate, Is.LessThanOrEqualTo(DateTime.UtcNow), "Join date should not be in future");
        Assert.That(student.Role, Is.EqualTo(UserRole.Student), "Role should be Student");
        
        // Phone can be null or have value
        if (!string.IsNullOrEmpty(student.Phone))
        {
            Assert.That(student.Phone, Does.Match(@"^\+?\d+"), "Phone should contain digits");
        }
    }

    [Test]
    public async Task GetStudents_ShouldReturnCorrectResponseHeaders()
    {
        // Act
        var response = await ApiContext.GetAsync("/api/students");
        
        // Assert
        await AssertSuccessfulResponse(response);
        
        var headers = response.Headers;
        Assert.That(headers.ContainsKey("content-type"), Is.True, "Response should have content-type header");
        Assert.That(headers["content-type"], Does.Contain("application/json"), "Content type should be JSON");
    }

    [Test]
    public async Task GetStudents_ShouldReturnValidJson()
    {
        // Act
        var response = await ApiContext.GetAsync("/api/students");
        await AssertSuccessfulResponse(response);
        
        var jsonContent = await response.TextAsync();
        
        // Assert
        Assert.That(jsonContent, Is.Not.Null.And.Not.Empty, "Response should contain JSON content");
        
        // Verify it's valid JSON by attempting to parse
        Assert.DoesNotThrow(() => JsonSerializer.Deserialize<object>(jsonContent), 
            "Response should be valid JSON");
    }

    #endregion

    #region GetStudent Tests

    [Test]
    public async Task GetStudent_WithValidId_ShouldReturnStudentDetails()
    {
        // Arrange - First get a student to test with
        var allStudentsResponse = await ApiContext.GetAsync("/api/students");
        await AssertSuccessfulResponse(allStudentsResponse);
        var students = await DeserializeResponse<List<StudentDto>>(allStudentsResponse);
        var testStudentId = students.First().Id;
        
        // Act
        var response = await ApiContext.GetAsync($"/api/students/{testStudentId}");
        
        // Assert
        await AssertSuccessfulResponse(response);
        
        var student = await DeserializeResponse<StudentDto>(response);
        
        Assert.That(student, Is.Not.Null, "Student should not be null");
        Assert.That(student.Id, Is.EqualTo(testStudentId), "Student ID should match requested ID");
        Assert.That(student.Name, Is.Not.Null.And.Not.Empty, "Student should have name");
        Assert.That(student.Email, Is.Not.Null.And.Not.Empty, "Student should have email");
        Assert.That(student.Role, Is.EqualTo(UserRole.Student), "Student should have Student role");
    }

    [Test]
    public async Task GetStudent_WithNonExistentId_ShouldReturn404()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        
        // Act
        var response = await ApiContext.GetAsync($"/api/students/{nonExistentId}");
        
        // Assert
        await AssertErrorResponse(response, HttpStatusCode.NotFound);
    }

    [Test]
    public async Task GetStudent_WithInvalidGuid_ShouldReturn404()
    {
        // Act
        var response = await ApiContext.GetAsync("/api/students/invalid-guid");
        
        // Assert - ASP.NET Core returns 404 for invalid route parameters
        await AssertErrorResponse(response, HttpStatusCode.NotFound);
    }

    #endregion

    #region CreateStudent Tests

    [Test]
    public async Task CreateStudent_WithValidData_ShouldCreateAndReturnStudent()
    {
        // Arrange
        var createDto = new CreateStudentDto
        {
            Email = $"test.student.{Guid.NewGuid().ToString("N")[..8]}@example.com",
            Password = "TestPassword123!",
            FirstName = "Test",
            LastName = "Student",
            Phone = "+48 123 456 789",
            Gender = "Male",
            DateOfBirth = DateTime.Now.AddYears(-25)
        };

        // Act
        var response = await ApiContext.PostAsync("/api/students", new APIRequestContextOptions
        {
            Data = JsonSerializer.Serialize(createDto, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            })
        });

        // Assert
        await AssertSuccessfulResponse(response, HttpStatusCode.Created);
        
        var student = await DeserializeResponse<StudentDto>(response);
        
        Assert.That(student, Is.Not.Null, "Created student should not be null");
        Assert.That(student.Id, Is.Not.EqualTo(Guid.Empty), "Created student should have valid ID");
        Assert.That(student.Name, Is.EqualTo($"{createDto.FirstName} {createDto.LastName}"), "Student name should match");
        Assert.That(student.Email, Is.EqualTo(createDto.Email), "Email should match");
        Assert.That(student.Phone, Is.EqualTo(createDto.Phone), "Phone should match");
        Assert.That(student.Role, Is.EqualTo(UserRole.Student), "Role should be Student");
        Assert.That(student.IsActive, Is.True, "New student should be active");
        Assert.That(student.JoinDate, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromMinutes(1)), "Join date should be current time");
    }

    [Test]
    public async Task CreateStudent_WithDuplicateEmail_ShouldReturn400()
    {
        // Arrange - First get an existing student's email
        var existingStudentsResponse = await ApiContext.GetAsync("/api/students");
        await AssertSuccessfulResponse(existingStudentsResponse);
        var existingStudents = await DeserializeResponse<List<StudentDto>>(existingStudentsResponse);
        var existingEmail = existingStudents.First().Email;

        var createDto = new CreateStudentDto
        {
            Email = existingEmail, // Duplicate email
            Password = "TestPassword123!",
            FirstName = "Duplicate",
            LastName = "Student",
            Phone = "+48 987 654 321"
        };

        // Act
        var response = await ApiContext.PostAsync("/api/students", new APIRequestContextOptions
        {
            Data = JsonSerializer.Serialize(createDto, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            })
        });

        // Assert
        await AssertErrorResponse(response, HttpStatusCode.BadRequest);
        
        var errorContent = await response.TextAsync();
        Assert.That(errorContent, Does.Contain("email").IgnoreCase, "Error should mention email conflict");
    }

    [Test]
    public async Task CreateStudent_WithInvalidEmail_ShouldReturn400()
    {
        // Arrange
        var createDto = new CreateStudentDto
        {
            Email = "invalid-email", // Invalid email format
            Password = "TestPassword123!",
            FirstName = "Test",
            LastName = "Student"
        };

        // Act
        var response = await ApiContext.PostAsync("/api/students", new APIRequestContextOptions
        {
            Data = JsonSerializer.Serialize(createDto, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            })
        });

        // Assert - Should return 400 for invalid data
        Assert.That(response.Status, Is.AnyOf(400, 422), "Should return error for invalid email");
    }

    [Test]
    public async Task CreateStudent_WithMissingRequiredFields_ShouldReturn400()
    {
        // Arrange - Send JSON with missing required fields directly
        var incompleteJson = @"{""email"": ""test@example.com""}";

        // Act
        var response = await ApiContext.PostAsync("/api/students", new APIRequestContextOptions
        {
            Data = incompleteJson
        });

        // Assert
        Assert.That(response.Status, Is.AnyOf(400, 422), "Should return error for missing required fields");
    }

    #endregion

    #region UpdateStudent Tests

    [Test]
    public async Task UpdateStudent_WithValidData_ShouldReturnUpdatedStudent()
    {
        // Arrange - First get an existing student
        var allStudentsResponse = await ApiContext.GetAsync("/api/students");
        await AssertSuccessfulResponse(allStudentsResponse);
        var students = await DeserializeResponse<List<StudentDto>>(allStudentsResponse);
        var testStudentId = students.First().Id;

        var updateDto = new UpdateStudentDto
        {
            FirstName = "Updated",
            LastName = "Name",
            Phone = "+48 999 888 777",
            Gender = "Female",
            DateOfBirth = DateTime.Now.AddYears(-30),
            IsActive = true
        };

        // Act
        var response = await ApiContext.PutAsync($"/api/students/{testStudentId}", new APIRequestContextOptions
        {
            Data = JsonSerializer.Serialize(updateDto, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            })
        });

        // Assert
        await AssertSuccessfulResponse(response);
        
        var updatedStudent = await DeserializeResponse<StudentDto>(response);
        
        Assert.That(updatedStudent, Is.Not.Null, "Updated student should not be null");
        Assert.That(updatedStudent.Id, Is.EqualTo(testStudentId), "Student ID should remain same");
        Assert.That(updatedStudent.Name, Is.EqualTo($"{updateDto.FirstName} {updateDto.LastName}"), "Name should be updated");
        Assert.That(updatedStudent.Phone, Is.EqualTo(updateDto.Phone), "Phone should be updated");
        Assert.That(updatedStudent.IsActive, Is.EqualTo(updateDto.IsActive), "IsActive should be updated");
        
        // Verify immutable fields are preserved
        var originalStudent = students.First();
        Assert.That(updatedStudent.Email, Is.EqualTo(originalStudent.Email), "Email should be preserved");
        Assert.That(updatedStudent.Role, Is.EqualTo(originalStudent.Role), "Role should be preserved");
        Assert.That(updatedStudent.JoinDate, Is.EqualTo(originalStudent.JoinDate), "Join date should be preserved");
    }

    [Test]
    public async Task UpdateStudent_WithNonExistentId_ShouldReturn404()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var updateDto = new UpdateStudentDto
        {
            FirstName = "Updated",
            LastName = "Name",
            IsActive = true
        };

        // Act
        var response = await ApiContext.PutAsync($"/api/students/{nonExistentId}", new APIRequestContextOptions
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

    #region DeleteStudent Tests

    [Test]
    public async Task DeleteStudent_WithValidId_ShouldReturn204()
    {
        // Arrange - First create a student to delete
        var createDto = new CreateStudentDto
        {
            Email = $"delete.test.{Guid.NewGuid().ToString("N")[..8]}@example.com",
            Password = "TestPassword123!",
            FirstName = "ToDelete",
            LastName = "Student",
            Phone = "+48 111 222 333"
        };

        var createResponse = await ApiContext.PostAsync("/api/students", new APIRequestContextOptions
        {
            Data = JsonSerializer.Serialize(createDto, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            })
        });
        await AssertSuccessfulResponse(createResponse, HttpStatusCode.Created);
        var createdStudent = await DeserializeResponse<StudentDto>(createResponse);

        // Act
        var deleteResponse = await ApiContext.DeleteAsync($"/api/students/{createdStudent.Id}");

        // Assert
        await AssertSuccessfulResponse(deleteResponse, HttpStatusCode.NoContent);
        
        // Verify student is actually deleted
        var getResponse = await ApiContext.GetAsync($"/api/students/{createdStudent.Id}");
        await AssertErrorResponse(getResponse, HttpStatusCode.NotFound);
    }

    [Test]
    public async Task DeleteStudent_WithNonExistentId_ShouldReturn404()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await ApiContext.DeleteAsync($"/api/students/{nonExistentId}");

        // Assert
        await AssertErrorResponse(response, HttpStatusCode.NotFound);
    }

    #endregion

    #region SearchStudents Tests

    [Test]
    public async Task SearchStudents_WithValidSearchTerm_ShouldReturnMatchingStudents()
    {
        // Arrange
        var searchTerm = "Anna"; // Based on seeded data

        // Act
        var response = await ApiContext.GetAsync($"/api/students/search?searchTerm={searchTerm}");

        // Assert
        await AssertSuccessfulResponse(response);
        
        var students = await DeserializeResponse<List<StudentSummaryDto>>(response);
        
        Assert.That(students, Is.Not.Null, "Search results should not be null");
        
        if (students.Any())
        {
            // Verify search results contain the search term
            foreach (var student in students)
            {
                var nameContainsSearchTerm = student.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
                var emailContainsSearchTerm = student.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
                
                Assert.That(nameContainsSearchTerm || emailContainsSearchTerm, Is.True,
                    $"Student {student.Name} ({student.Email}) should contain search term '{searchTerm}'");
            }
        }
    }

    [Test]
    public async Task SearchStudents_WithEmailSearchTerm_ShouldReturnMatchingStudents()
    {
        // Arrange - First get a student to search for
        var allStudentsResponse = await ApiContext.GetAsync("/api/students");
        await AssertSuccessfulResponse(allStudentsResponse);
        var students = await DeserializeResponse<List<StudentDto>>(allStudentsResponse);
        var firstStudent = students.First();
        var emailPart = firstStudent.Email.Split('@')[0]; // Get part before @

        // Act
        var response = await ApiContext.GetAsync($"/api/students/search?searchTerm={emailPart}");

        // Assert
        await AssertSuccessfulResponse(response);
        
        var searchResults = await DeserializeResponse<List<StudentSummaryDto>>(response);
        
        Assert.That(searchResults, Is.Not.Null, "Search results should not be null");
        Assert.That(searchResults.Count, Is.GreaterThan(0), "Should find at least one student");
        
        // Verify the original student is in results
        var foundStudent = searchResults.FirstOrDefault(s => s.Id == firstStudent.Id);
        Assert.That(foundStudent, Is.Not.Null, "Original student should be found in search results");
    }

    [Test]
    public async Task SearchStudents_WithTooShortSearchTerm_ShouldReturn400()
    {
        // Arrange
        var shortSearchTerm = "A"; // Less than 2 characters

        // Act
        var response = await ApiContext.GetAsync($"/api/students/search?searchTerm={shortSearchTerm}");

        // Assert
        await AssertErrorResponse(response, HttpStatusCode.BadRequest);
        
        var errorContent = await response.TextAsync();
        Assert.That(errorContent, Does.Contain("2 characters").IgnoreCase, 
            "Error should mention minimum character requirement");
    }

    [Test]
    public async Task SearchStudents_WithEmptySearchTerm_ShouldReturn400()
    {
        // Act
        var response = await ApiContext.GetAsync("/api/students/search?searchTerm=");

        // Assert
        await AssertErrorResponse(response, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task SearchStudents_WithNonExistentSearchTerm_ShouldReturnEmptyList()
    {
        // Arrange
        var nonExistentSearchTerm = "XYZNonExistentStudent123";

        // Act
        var response = await ApiContext.GetAsync($"/api/students/search?searchTerm={nonExistentSearchTerm}");

        // Assert
        await AssertSuccessfulResponse(response);
        
        var students = await DeserializeResponse<List<StudentSummaryDto>>(response);
        
        Assert.That(students, Is.Not.Null, "Search results should not be null");
        Assert.That(students.Count, Is.EqualTo(0), "Should return empty list for non-existent search term");
    }

    #endregion

    #region Integration Tests

    [Test]
    public async Task StudentLifecycle_CreateUpdateDeleteFlow_ShouldWorkCorrectly()
    {
        // Create
        var createDto = new CreateStudentDto
        {
            Email = $"lifecycle.test.{Guid.NewGuid().ToString("N")[..8]}@example.com",
            Password = "TestPassword123!",
            FirstName = "Lifecycle",
            LastName = "Test",
            Phone = "+48 123 999 888"
        };

        var createResponse = await ApiContext.PostAsync("/api/students", new APIRequestContextOptions
        {
            Data = JsonSerializer.Serialize(createDto, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            })
        });
        await AssertSuccessfulResponse(createResponse, HttpStatusCode.Created);
        var createdStudent = await DeserializeResponse<StudentDto>(createResponse);

        // Verify creation
        Assert.That(createdStudent.Name, Is.EqualTo($"{createDto.FirstName} {createDto.LastName}"));
        Assert.That(createdStudent.Email, Is.EqualTo(createDto.Email));

        // Update
        var updateDto = new UpdateStudentDto
        {
            FirstName = "Updated",
            LastName = "Lifecycle",
            Phone = "+48 999 888 777",
            IsActive = true
        };

        var updateResponse = await ApiContext.PutAsync($"/api/students/{createdStudent.Id}", new APIRequestContextOptions
        {
            Data = JsonSerializer.Serialize(updateDto, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            })
        });
        await AssertSuccessfulResponse(updateResponse);
        var updatedStudent = await DeserializeResponse<StudentDto>(updateResponse);

        // Verify update
        Assert.That(updatedStudent.Name, Is.EqualTo($"{updateDto.FirstName} {updateDto.LastName}"));
        Assert.That(updatedStudent.Phone, Is.EqualTo(updateDto.Phone));

        // Delete
        var deleteResponse = await ApiContext.DeleteAsync($"/api/students/{createdStudent.Id}");
        await AssertSuccessfulResponse(deleteResponse, HttpStatusCode.NoContent);

        // Verify deletion
        var getResponse = await ApiContext.GetAsync($"/api/students/{createdStudent.Id}");
        await AssertErrorResponse(getResponse, HttpStatusCode.NotFound);
    }

    #endregion
}
