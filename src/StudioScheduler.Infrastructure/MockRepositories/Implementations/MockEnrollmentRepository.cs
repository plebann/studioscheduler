using System.Text.Json;
using StudioScheduler.Core.Interfaces.Repositories;
using StudioScheduler.Core.Models;

namespace StudioScheduler.Infrastructure.MockRepositories.Implementations;

public class MockEnrollmentRepository : IEnrollmentRepository
{
    private readonly string _dataPath;
    private List<Enrollment>? _enrollments;

    public MockEnrollmentRepository()
    {
        _dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MockRepositories", "Data", "enrollments.json");
    }

    private async Task<List<Enrollment>> LoadEnrollmentsAsync()
    {
        if (_enrollments != null)
            return _enrollments;

        Console.WriteLine($"Looking for enrollments file at: {_dataPath}");
        Console.WriteLine($"File exists: {File.Exists(_dataPath)}");

        if (!File.Exists(_dataPath))
        {
            Console.WriteLine("Enrollments file not found, returning empty list");
            _enrollments = new List<Enrollment>();
            return _enrollments;
        }

        try
        {
            var json = await File.ReadAllTextAsync(_dataPath);
            var data = JsonSerializer.Deserialize<EnrollmentData>(json, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });

            _enrollments = data?.Enrollments?.Select(e => new Enrollment
            {
                Id = Guid.Parse(e.Id),
                StudentId = Guid.Parse(e.StudentId),
                ScheduleId = Guid.Parse(e.ScheduleId),
                EnrolledDate = e.EnrolledDate,
                IsActive = e.IsActive,
                CreatedAt = e.EnrolledDate
            }).ToList() ?? new List<Enrollment>();

            Console.WriteLine($"Loaded {_enrollments.Count} enrollments from JSON");
            foreach (var enrollment in _enrollments.Take(3))
            {
                Console.WriteLine($"Enrollment: {enrollment.Id}, Student: {enrollment.StudentId}, Schedule: {enrollment.ScheduleId}, Active: {enrollment.IsActive}");
            }

            return _enrollments;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading enrollments: {ex.Message}");
            _enrollments = new List<Enrollment>();
            return _enrollments;
        }
    }

    public async Task<IEnumerable<Enrollment>> GetAllAsync()
    {
        var enrollments = await LoadEnrollmentsAsync();
        return enrollments.Where(e => e.IsActive);
    }

    public async Task<Enrollment?> GetByIdAsync(Guid id)
    {
        var enrollments = await LoadEnrollmentsAsync();
        return enrollments.FirstOrDefault(e => e.Id == id);
    }

    public async Task<IEnumerable<Enrollment>> GetByStudentIdAsync(Guid studentId)
    {
        var enrollments = await LoadEnrollmentsAsync();
        return enrollments.Where(e => e.StudentId == studentId && e.IsActive);
    }

    public async Task<IEnumerable<Enrollment>> GetByScheduleIdAsync(Guid scheduleId)
    {
        var enrollments = await LoadEnrollmentsAsync();
        var matchingEnrollments = enrollments.Where(e => e.ScheduleId == scheduleId && e.IsActive).ToList();
        Console.WriteLine($"GetByScheduleIdAsync called for schedule {scheduleId}");
        Console.WriteLine($"Total enrollments: {enrollments.Count}");
        Console.WriteLine($"Matching enrollments for schedule {scheduleId}: {matchingEnrollments.Count}");
        return matchingEnrollments;
    }

    public async Task<Enrollment> CreateAsync(Enrollment enrollment)
    {
        var enrollments = await LoadEnrollmentsAsync();
        enrollment.Id = Guid.NewGuid();
        enrollments.Add(enrollment);
        await SaveEnrollmentsAsync(enrollments);
        return enrollment;
    }

    public async Task<Enrollment> UpdateAsync(Enrollment enrollment)
    {
        var enrollments = await LoadEnrollmentsAsync();
        var index = enrollments.FindIndex(e => e.Id == enrollment.Id);
        if (index >= 0)
        {
            enrollments[index] = enrollment;
            await SaveEnrollmentsAsync(enrollments);
        }
        return enrollment;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var enrollments = await LoadEnrollmentsAsync();
        var enrollment = enrollments.FirstOrDefault(e => e.Id == id);
        if (enrollment != null)
        {
            enrollment.IsActive = false;
            await SaveEnrollmentsAsync(enrollments);
            return true;
        }
        return false;
    }

    private async Task SaveEnrollmentsAsync(List<Enrollment> enrollments)
    {
        _enrollments = enrollments;
        // In a real implementation, we would save back to JSON
        // For now, just keep in memory
        await Task.CompletedTask;
    }

    // Helper classes for JSON deserialization
    private class EnrollmentData
    {
        public List<EnrollmentJson>? Enrollments { get; set; }
    }

    private class EnrollmentJson
    {
        public string Id { get; set; } = "";
        public string StudentId { get; set; } = "";
        public string ScheduleId { get; set; } = "";
        public string ClassName { get; set; } = "";
        public string DayOfWeek { get; set; } = "";
        public DateTime StartTime { get; set; }
        public DateTime EnrolledDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
