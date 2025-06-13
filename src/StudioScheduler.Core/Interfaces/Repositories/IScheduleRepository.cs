using StudioScheduler.Core.Models;

namespace StudioScheduler.Core.Interfaces.Repositories;

public interface IScheduleRepository
{
    Task<Schedule?> GetByIdAsync(Guid id);
    Task<IEnumerable<Schedule>> GetAllAsync();
    Task<Schedule> AddAsync(Schedule schedule);
    Task<Schedule> UpdateAsync(Schedule schedule);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<IEnumerable<Schedule>> GetByLocationAsync(Guid locationId);
    Task<IEnumerable<Schedule>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Schedule>> GetByDanceClassAsync(Guid danceClassId);
    Task<IEnumerable<Schedule>> GetActiveSchedulesAsync();
    Task<IEnumerable<Schedule>> GetByInstructorAsync(Guid instructorId);
    Task<bool> IsTimeSlotAvailableAsync(Guid roomId, DateTime startTime, TimeSpan duration);
    Task<int> GetAvailableSpotsAsync(Guid scheduleId);
    Task<bool> HasScheduleConflictAsync(Guid roomId, DateTime startTime, TimeSpan duration, Guid? excludeScheduleId = null);
    Task<bool> CancelScheduleAsync(Guid scheduleId);
    Task SaveChangesAsync();
}
