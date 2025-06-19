using StudioScheduler.Core.Models;

namespace StudioScheduler.Core.Interfaces.Repositories;

public interface IDanceClassRepository
{
    Task<DanceClass?> GetByIdAsync(Guid id);
    Task<IEnumerable<DanceClass>> GetAllAsync();
    Task<DanceClass> AddAsync(DanceClass danceClass);
    Task<DanceClass> UpdateAsync(DanceClass danceClass);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<IEnumerable<DanceClass>> GetByStyleAsync(string style);
    Task<IEnumerable<Schedule>> GetSchedulesAsync(Guid classId);
    Task<int> GetCurrentEnrollmentAsync(Guid classId);
    Task<bool> IsInstructorAvailableAsync(Guid instructorId, DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan duration);
    Task SaveChangesAsync();
    Task<DanceClass?> GetByNameAsync(string name);
}
