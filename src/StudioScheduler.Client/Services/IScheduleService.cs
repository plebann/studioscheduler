using StudioScheduler.Shared.Dtos;

namespace StudioScheduler.Client.Services;

public interface IScheduleService
{
    Task<List<ScheduleDto>> GetAllSchedulesAsync();
    Task<List<ScheduleSelectionDto>> GetSchedulesForSelectionAsync();
}
