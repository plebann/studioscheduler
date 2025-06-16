using StudioScheduler.Core.Models;

namespace StudioScheduler.Core.Interfaces.Repositories;

public interface IAttendanceRepository
{
    Task<IEnumerable<Attendance>> GetAllAsync();
    Task<Attendance?> GetByIdAsync(Guid id);
    Task<IEnumerable<Attendance>> GetByStudentIdAsync(Guid studentId);
    Task<IEnumerable<Attendance>> GetByScheduleIdAsync(Guid scheduleId);
    Task<IEnumerable<Attendance>> GetByStudentAndScheduleAsync(Guid studentId, Guid scheduleId);
    Task<IEnumerable<Attendance>> GetByPassIdAsync(Guid passId);
    Task<Attendance?> GetByStudentScheduleAndDateAsync(Guid studentId, Guid scheduleId, DateTime classDate);
    Task<Attendance> CreateAsync(Attendance attendance);
    Task<Attendance> UpdateAsync(Attendance attendance);
    Task<bool> DeleteAsync(Guid id);
}
