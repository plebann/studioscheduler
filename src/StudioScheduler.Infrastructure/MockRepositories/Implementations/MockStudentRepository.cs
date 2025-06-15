using System.Text.Json;
using StudioScheduler.Core.Interfaces.Repositories;
using StudioScheduler.Core.Models;
using StudioScheduler.Core.Enums;

namespace StudioScheduler.Infrastructure.MockRepositories.Implementations;

public class MockStudentRepository : IStudentRepository
{
    private readonly string _dataPath;
    private List<Student>? _students;

    public MockStudentRepository()
    {
        _dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MockRepositories", "Data", "students.json");
    }

    private async Task<List<Student>> LoadStudentsAsync()
    {
        if (_students != null)
            return _students;

        if (!File.Exists(_dataPath))
        {
            _students = new List<Student>();
            return _students;
        }

        try
        {
            var json = await File.ReadAllTextAsync(_dataPath);
            var data = JsonSerializer.Deserialize<StudentData>(json, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });

            _students = data?.Students?.Select(s => 
            {
                Console.WriteLine($"Parsing student with ID: '{s.Id}'");
                var student = new Student
                {
                    Id = Guid.Parse(s.Id),
                    Email = s.Email,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    PasswordHash = "mock_hash", // Mock password hash
                    Phone = s.Phone,
                    DateOfBirth = s.DateOfBirth,
                    Gender = s.Gender,
                    Role = UserRole.Student,
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt
                };

                // Add the current pass if it exists
                if (s.CurrentPass != null)
                {
                    Console.WriteLine($"Parsing pass with ID: '{s.CurrentPass.Id}' and Type: '{s.CurrentPass.Type}'");
                    var pass = new Pass
                    {
                        Id = Guid.Parse(s.CurrentPass.Id),
                        UserId = student.Id,
                        User = student,
                        StartDate = s.CurrentPass.StartDate,
                        EndDate = s.CurrentPass.EndDate,
                        Type = Enum.Parse<PassType>(s.CurrentPass.Type),
                        TotalClasses = s.CurrentPass.TotalClasses,
                        RemainingClasses = s.CurrentPass.RemainingClasses,
                        ClassesPerWeek = s.CurrentPass.ClassesPerWeek,
                        IsActive = s.CurrentPass.IsActive,
                        CreatedAt = DateTime.UtcNow
                    };
                    
                    student.Passes.Add(pass);
                }

                return student;
            }).ToList() ?? new List<Student>();

            return _students;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading students: {ex.Message}");
            _students = new List<Student>();
            return _students;
        }
    }

    public async Task<IEnumerable<Student>> GetAllAsync()
    {
        var students = await LoadStudentsAsync();
        return students.Where(s => s.IsActive);
    }

    public async Task<Student?> GetByIdAsync(Guid id)
    {
        var students = await LoadStudentsAsync();
        return students.FirstOrDefault(s => s.Id == id);
    }

    public async Task<IEnumerable<Student>> GetByScheduleIdAsync(Guid scheduleId)
    {
        // This would need to be implemented with enrollment data
        // For now, return empty collection
        await Task.CompletedTask;
        return new List<Student>();
    }

    public async Task<Student> CreateAsync(Student student)
    {
        var students = await LoadStudentsAsync();
        student.Id = Guid.NewGuid();
        students.Add(student);
        await SaveStudentsAsync(students);
        return student;
    }

    public async Task<Student> UpdateAsync(Student student)
    {
        var students = await LoadStudentsAsync();
        var index = students.FindIndex(s => s.Id == student.Id);
        if (index >= 0)
        {
            students[index] = student;
            await SaveStudentsAsync(students);
        }
        return student;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var students = await LoadStudentsAsync();
        var student = students.FirstOrDefault(s => s.Id == id);
        if (student != null)
        {
            student.IsActive = false;
            await SaveStudentsAsync(students);
            return true;
        }
        return false;
    }

    private async Task SaveStudentsAsync(List<Student> students)
    {
        _students = students;
        // In a real implementation, we would save back to JSON
        // For now, just keep in memory
        await Task.CompletedTask;
    }

    // Helper classes for JSON deserialization
    private class StudentData
    {
        public List<StudentJson>? Students { get; set; }
    }

    private class StudentJson
    {
        public string Id { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public PassJson? CurrentPass { get; set; }
    }

    private class PassJson
    {
        public string Id { get; set; } = "";
        public string Type { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalClasses { get; set; }
        public int RemainingClasses { get; set; }
        public int ClassesPerWeek { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public bool IsExpired { get; set; }
    }
}
