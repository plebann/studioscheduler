using StudioScheduler.Core.Interfaces.Repositories;
using StudioScheduler.Core.Interfaces.Services;
using StudioScheduler.Core.Models;

namespace StudioScheduler.Infrastructure.Services;

public class ScheduleService : IScheduleService
{
    private readonly IScheduleRepository _scheduleRepository;

    public ScheduleService(IScheduleRepository scheduleRepository)
    {
        _scheduleRepository = scheduleRepository;
    }

    public async Task<IEnumerable<Schedule>> GetAllAsync()
        => await _scheduleRepository.GetAllAsync();

    public async Task<Schedule?> GetByIdAsync(Guid id)
        => await _scheduleRepository.GetByIdAsync(id);

    public async Task<Schedule> CreateAsync(Schedule schedule)
        => await _scheduleRepository.AddAsync(schedule);

    public async Task<Schedule> UpdateAsync(Schedule schedule)
        => await _scheduleRepository.UpdateAsync(schedule);

    public async Task<bool> DeleteAsync(Guid id)
        => await _scheduleRepository.DeleteAsync(id);

    public async Task<IEnumerable<Schedule>> GetByLocationAsync(Guid locationId)
        => await _scheduleRepository.GetByLocationAsync(locationId);

    public async Task<IEnumerable<Schedule>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        => await _scheduleRepository.GetByDateRangeAsync(startDate, endDate);

    public async Task<IEnumerable<Schedule>> GetByDanceClassAsync(Guid danceClassId)
        => await _scheduleRepository.GetByDanceClassAsync(danceClassId);

    public async Task<bool> ExistsAsync(Guid id)
        => await _scheduleRepository.ExistsAsync(id);

    public async Task<bool> CancelClassAsync(Guid scheduleId)
        => await _scheduleRepository.CancelScheduleAsync(scheduleId);

    public async Task<bool> IsTimeSlotAvailableAsync(Guid roomId, DateTime startTime, TimeSpan duration)
        => await _scheduleRepository.IsTimeSlotAvailableAsync(roomId, startTime, duration);

    public async Task<IEnumerable<Schedule>> GetActiveSchedulesAsync()
        => await _scheduleRepository.GetActiveSchedulesAsync();

    public async Task<IEnumerable<Schedule>> GetSchedulesByInstructorAsync(Guid instructorId)
        => await _scheduleRepository.GetByInstructorAsync(instructorId);

    public async Task<int> GetAvailableSpotsAsync(Guid scheduleId)
        => await _scheduleRepository.GetAvailableSpotsAsync(scheduleId);

    public async Task<bool> IsScheduleConflictAsync(Guid roomId, DateTime startTime, TimeSpan duration, Guid? excludeScheduleId = null)
        => await _scheduleRepository.HasScheduleConflictAsync(roomId, startTime, duration, excludeScheduleId);
}
