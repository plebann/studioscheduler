using StudioScheduler.Core.Models;

namespace StudioScheduler.Core.Interfaces.Services;

public interface IDanceClassService
{
    Task<DanceClass?> GetByIdAsync(Guid id);
    Task<IEnumerable<DanceClass>> GetAllAsync();
    Task<IEnumerable<DanceClass>> GetByStyleAsync(string style);
    Task<IEnumerable<DanceClass>> GetByLevelAsync(string level);
    Task<IEnumerable<DanceClass>> GetByInstructorAsync(Guid instructorId);
    Task<DanceClass> CreateAsync(DanceClass danceClass);
    Task<DanceClass> UpdateAsync(DanceClass danceClass);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<IEnumerable<Schedule>> GetClassSchedulesAsync(Guid classId);
    Task<IEnumerable<DanceClass>> GetClassesByRoomAsync(Guid roomId);
    Task<bool> IsInstructorAvailableAsync(Guid instructorId, DateTime startTime, TimeSpan duration);
    Task<int> GetCurrentEnrollmentAsync(Guid classId);
}
