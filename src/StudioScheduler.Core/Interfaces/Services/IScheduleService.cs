using StudioScheduler.Core.Models;

namespace StudioScheduler.Core.Interfaces.Services;

public interface IScheduleService
{
    Task<Schedule?> GetByIdAsync(Guid id);
    Task<IEnumerable<Schedule>> GetAllAsync();
    Task<Schedule> CreateAsync(Schedule schedule);
    Task<Schedule> UpdateAsync(Schedule schedule);
    Task<bool> DeleteAsync(Guid id);
}
