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
}
