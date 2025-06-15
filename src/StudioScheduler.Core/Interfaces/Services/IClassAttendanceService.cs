using StudioScheduler.Core.Models;

namespace StudioScheduler.Core.Interfaces.Services;

public interface IClassAttendanceService
{
    Task<Schedule?> GetClassAttendanceAsync(Guid scheduleId);
    Task<Attendance> MarkAttendanceAsync(Guid scheduleId, Guid studentId, bool isPresent, string? notes = null);
    Task<IEnumerable<Student>> SearchStudentsAsync(string searchTerm);
    Task<IEnumerable<Attendance>> GetAttendanceHistoryAsync(Guid studentId, Guid scheduleId, int weeks = 3);
}
