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
    Task<IEnumerable<DanceClass>> GetByLevelAsync(string level);
    Task<IEnumerable<DanceClass>> GetByInstructorAsync(Guid instructorId);
    Task<IEnumerable<DanceClass>> GetByRoomAsync(Guid roomId);
    Task<IEnumerable<Schedule>> GetSchedulesAsync(Guid classId);
    Task<int> GetCurrentEnrollmentAsync(Guid classId);
    Task<bool> IsInstructorAvailableAsync(Guid instructorId, DateTime startTime, TimeSpan duration);
    Task SaveChangesAsync();
}
