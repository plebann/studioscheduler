using System.Net.Http.Json;
using StudioScheduler.Shared.Dtos;

namespace StudioScheduler.Client.Services;

public interface IScheduleService
{
    Task<List<ScheduleDto>> GetAllSchedulesAsync();
    Task<List<ScheduleSelectionDto>> GetSchedulesForSelectionAsync();
}

public class ScheduleService : IScheduleService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ScheduleService> _logger;

    public ScheduleService(HttpClient httpClient, ILogger<ScheduleService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<ScheduleDto>> GetAllSchedulesAsync()
    {
        try
        {
            _logger.LogInformation("Fetching all schedules from API");
            var response = await _httpClient.GetFromJsonAsync<List<ScheduleDto>>("api/schedules");
            return response ?? new List<ScheduleDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching schedules");
            return new List<ScheduleDto>();
        }
    }

    public async Task<List<ScheduleSelectionDto>> GetSchedulesForSelectionAsync()
    {
        try
        {
            _logger.LogInformation("Fetching schedules for selection");
            var schedules = await GetAllSchedulesAsync();
            
            return schedules.Where(s => s.IsActive && !s.IsCancelled)
                .Select(s => new ScheduleSelectionDto
                {
                    ScheduleId = s.Id,
                    DisplayText = FormatScheduleDisplayText(s),
                    DayOfWeek = s.DayOfWeek,
                    StartTime = s.StartTime,
                    DanceClass = s.DanceClassName ?? "Unknown",
                    Level = s.Level,
                    InstructorName = s.InstructorName,
                    IsSelected = false
                })
                .OrderBy(s => s.DayOfWeek)
                .ThenBy(s => s.StartTime)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching schedules for selection");
            return new List<ScheduleSelectionDto>();
        }
    }

    private static string FormatScheduleDisplayText(ScheduleDto schedule)
    {
        var timeStr = schedule.StartTime.ToString(@"hh\:mm");
        var danceClass = schedule.DanceClassName ?? "Unknown";
        var level = schedule.Level;
        var instructor = !string.IsNullOrEmpty(schedule.InstructorName) 
            ? $" ({schedule.InstructorName})" 
            : "";
        
        return $"{schedule.DayOfWeek} {timeStr} - {danceClass} {level}{instructor}";
    }
}
