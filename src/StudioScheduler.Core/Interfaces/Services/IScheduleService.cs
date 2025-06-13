using StudioScheduler.Core.Models;

namespace StudioScheduler.Core.Interfaces.Services;

public interface IScheduleService
{
    Task<Schedule?> GetByIdAsync(Guid id);
    Task<IEnumerable<Schedule>> GetAllAsync();
    Task<IEnumerable<Schedule>> GetByLocationAsync(Guid locationId);
    Task<IEnumerable<Schedule>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Schedule>> GetByDanceClassAsync(Guid danceClassId);
    Task<Schedule> CreateAsync(Schedule schedule);
    Task<Schedule> UpdateAsync(Schedule schedule);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> CancelClassAsync(Guid scheduleId);
    Task<bool> IsTimeSlotAvailableAsync(Guid roomId, DateTime startTime, TimeSpan duration);
    Task<IEnumerable<Schedule>> GetActiveSchedulesAsync();
    Task<IEnumerable<Schedule>> GetSchedulesByInstructorAsync(Guid instructorId);
    Task<int> GetAvailableSpotsAsync(Guid scheduleId);
    Task<bool> IsScheduleConflictAsync(Guid roomId, DateTime startTime, TimeSpan duration, Guid? excludeScheduleId = null);
}
