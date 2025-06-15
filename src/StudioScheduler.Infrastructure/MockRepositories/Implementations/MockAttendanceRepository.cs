using System.Text.Json;
using StudioScheduler.Core.Interfaces.Repositories;
using StudioScheduler.Core.Models;

namespace StudioScheduler.Infrastructure.MockRepositories.Implementations;

public class MockAttendanceRepository : IAttendanceRepository
{
    private readonly string _dataPath;
    private List<Attendance>? _attendanceRecords;

    public MockAttendanceRepository()
    {
        _dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MockRepositories", "Data", "attendance.json");
    }

    private async Task<List<Attendance>> LoadAttendanceAsync()
    {
        if (_attendanceRecords != null)
            return _attendanceRecords;

        if (!File.Exists(_dataPath))
        {
            _attendanceRecords = new List<Attendance>();
            return _attendanceRecords;
        }

        try
        {
            var json = await File.ReadAllTextAsync(_dataPath);
            var data = JsonSerializer.Deserialize<AttendanceData>(json, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });

            _attendanceRecords = data?.AttendanceHistory?.Select(a => new Attendance
            {
                Id = Guid.Parse(a.Id.Replace("attendance-", "")),
                StudentId = Guid.Parse(a.StudentId.Replace("student-", "")),
                ScheduleId = Guid.Parse(a.ScheduleId),
                ClassDate = a.ClassDate,
                WasPresent = a.WasPresent,
                PassUsed = !string.IsNullOrEmpty(a.PassUsed) ? Guid.Parse(a.PassUsed.Replace("pass-", "")) : null,
                PassClassNumber = a.PassClassNumber,
                CreatedAt = a.CreatedAt ?? DateTime.UtcNow
            }).ToList() ?? new List<Attendance>();

            return _attendanceRecords;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading attendance: {ex.Message}");
            _attendanceRecords = new List<Attendance>();
            return _attendanceRecords;
        }
    }

    public async Task<IEnumerable<Attendance>> GetAllAsync()
    {
        var attendance = await LoadAttendanceAsync();
        return attendance;
    }

    public async Task<Attendance?> GetByIdAsync(Guid id)
    {
        var attendance = await LoadAttendanceAsync();
        return attendance.FirstOrDefault(a => a.Id == id);
    }

    public async Task<IEnumerable<Attendance>> GetByStudentIdAsync(Guid studentId)
    {
        var attendance = await LoadAttendanceAsync();
        return attendance.Where(a => a.StudentId == studentId);
    }

    public async Task<IEnumerable<Attendance>> GetByScheduleIdAsync(Guid scheduleId)
    {
        var attendance = await LoadAttendanceAsync();
        return attendance.Where(a => a.ScheduleId == scheduleId);
    }

    public async Task<IEnumerable<Attendance>> GetByStudentAndScheduleAsync(Guid studentId, Guid scheduleId)
    {
        var attendance = await LoadAttendanceAsync();
        return attendance.Where(a => a.StudentId == studentId && a.ScheduleId == scheduleId)
                        .OrderByDescending(a => a.ClassDate);
    }

    public async Task<Attendance?> GetByStudentScheduleAndDateAsync(Guid studentId, Guid scheduleId, DateTime classDate)
    {
        var attendance = await LoadAttendanceAsync();
        return attendance.FirstOrDefault(a => 
            a.StudentId == studentId && 
            a.ScheduleId == scheduleId && 
            a.ClassDate.Date == classDate.Date);
    }

    public async Task<Attendance> CreateAsync(Attendance attendance)
    {
        var attendanceRecords = await LoadAttendanceAsync();
        attendance.Id = Guid.NewGuid();
        attendanceRecords.Add(attendance);
        await SaveAttendanceAsync(attendanceRecords);
        return attendance;
    }

    public async Task<Attendance> UpdateAsync(Attendance attendance)
    {
        var attendanceRecords = await LoadAttendanceAsync();
        var index = attendanceRecords.FindIndex(a => a.Id == attendance.Id);
        if (index >= 0)
        {
            attendanceRecords[index] = attendance;
            await SaveAttendanceAsync(attendanceRecords);
        }
        return attendance;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var attendanceRecords = await LoadAttendanceAsync();
        var attendance = attendanceRecords.FirstOrDefault(a => a.Id == id);
        if (attendance != null)
        {
            attendanceRecords.Remove(attendance);
            await SaveAttendanceAsync(attendanceRecords);
            return true;
        }
        return false;
    }

    private async Task SaveAttendanceAsync(List<Attendance> attendanceRecords)
    {
        _attendanceRecords = attendanceRecords;
        // In a real implementation, we would save back to JSON
        // For now, just keep in memory
        await Task.CompletedTask;
    }

    // Helper classes for JSON deserialization
    private class AttendanceData
    {
        public List<AttendanceJson>? AttendanceHistory { get; set; }
    }

    private class AttendanceJson
    {
        public string Id { get; set; } = "";
        public string StudentId { get; set; } = "";
        public string ScheduleId { get; set; } = "";
        public string ClassName { get; set; } = "";
        public DateTime ClassDate { get; set; }
        public int WeekOffset { get; set; }
        public bool WasPresent { get; set; }
        public string? PassUsed { get; set; }
        public int PassClassNumber { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
