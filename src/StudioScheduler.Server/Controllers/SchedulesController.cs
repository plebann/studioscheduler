using Microsoft.AspNetCore.Mvc;
using StudioScheduler.Core.Interfaces.Services;
using StudioScheduler.Core.Models;
using StudioScheduler.Shared.Dtos;

namespace StudioScheduler.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SchedulesController : ControllerBase
{
    private readonly IScheduleService _scheduleService;
    private readonly IDanceClassService _danceClassService;

    public SchedulesController(IScheduleService scheduleService, IDanceClassService danceClassService)
    {
        _scheduleService = scheduleService;
        _danceClassService = danceClassService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScheduleSummaryDto>>> GetSchedules()
    {
        var schedules = await _scheduleService.GetAllAsync();
        var summaries = schedules.Select(s => new ScheduleSummaryDto
        {
            Id = s.Id,
            Name = s.Name,
            LocationName = s.Location?.Name,
            DanceClassName = s.DanceClass?.Name,
            StartTime = s.StartTime,
            Duration = s.Duration,
            IsActive = s.IsActive,
            IsCancelled = s.IsCancelled
        });
        
        return Ok(summaries);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ScheduleDto>> GetSchedule(Guid id)
    {
        var schedule = await _scheduleService.GetByIdAsync(id);
        if (schedule == null)
        {
            return NotFound();
        }

        var scheduleDto = new ScheduleDto
        {
            Id = schedule.Id,
            Name = schedule.Name,
            LocationId = schedule.LocationId,
            LocationName = schedule.Location?.Name,
            DanceClassId = schedule.DanceClassId,
            DanceClassName = schedule.DanceClass?.Name,
            StartTime = schedule.StartTime,
            Duration = schedule.Duration,
            IsRecurring = schedule.IsRecurring,
            RecurrencePattern = schedule.RecurrencePattern,
            RecurrenceEndDate = schedule.RecurrenceEndDate,
            IsCancelled = schedule.IsCancelled,
            EffectiveFrom = schedule.EffectiveFrom,
            EffectiveTo = schedule.EffectiveTo,
            IsActive = schedule.IsActive,
            CreatedAt = schedule.CreatedAt,
            UpdatedAt = schedule.UpdatedAt
        };

        return Ok(scheduleDto);
    }

    [HttpPost]
    public async Task<ActionResult<ScheduleDto>> CreateSchedule(CreateScheduleDto createDto)
    {
        var schedule = new Schedule
        {
            Name = createDto.Name,
            LocationId = createDto.LocationId,
            DanceClassId = createDto.DanceClassId,
            StartTime = createDto.StartTime,
            Duration = createDto.Duration,
            IsRecurring = createDto.IsRecurring,
            RecurrencePattern = createDto.RecurrencePattern,
            RecurrenceEndDate = createDto.RecurrenceEndDate,
            EffectiveFrom = createDto.EffectiveFrom,
            EffectiveTo = createDto.EffectiveTo,
            IsActive = true,
            IsCancelled = false
        };

        var created = await _scheduleService.CreateAsync(schedule);
        
        var scheduleDto = new ScheduleDto
        {
            Id = created.Id,
            Name = created.Name,
            LocationId = created.LocationId,
            LocationName = created.Location?.Name,
            DanceClassId = created.DanceClassId,
            DanceClassName = created.DanceClass?.Name,
            StartTime = created.StartTime,
            Duration = created.Duration,
            IsRecurring = created.IsRecurring,
            RecurrencePattern = created.RecurrencePattern,
            RecurrenceEndDate = created.RecurrenceEndDate,
            IsCancelled = created.IsCancelled,
            EffectiveFrom = created.EffectiveFrom,
            EffectiveTo = created.EffectiveTo,
            IsActive = created.IsActive,
            CreatedAt = created.CreatedAt,
            UpdatedAt = created.UpdatedAt
        };

        return CreatedAtAction(nameof(GetSchedule), new { id = scheduleDto.Id }, scheduleDto);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ScheduleDto>> UpdateSchedule(Guid id, UpdateScheduleDto updateDto)
    {
        var existingSchedule = await _scheduleService.GetByIdAsync(id);
        if (existingSchedule == null)
        {
            return NotFound();
        }

        var updatedSchedule = new Schedule
        {
            Id = existingSchedule.Id,
            Name = updateDto.Name,
            LocationId = existingSchedule.LocationId,
            DanceClassId = existingSchedule.DanceClassId,
            StartTime = updateDto.StartTime,
            Duration = updateDto.Duration,
            IsRecurring = updateDto.IsRecurring,
            RecurrencePattern = updateDto.RecurrencePattern,
            RecurrenceEndDate = updateDto.RecurrenceEndDate,
            EffectiveFrom = updateDto.EffectiveFrom,
            EffectiveTo = updateDto.EffectiveTo,
            IsActive = updateDto.IsActive,
            IsCancelled = updateDto.IsCancelled,
            CreatedAt = existingSchedule.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        var updated = await _scheduleService.UpdateAsync(updatedSchedule);

        var scheduleDto = new ScheduleDto
        {
            Id = updated.Id,
            Name = updated.Name,
            LocationId = updated.LocationId,
            LocationName = updated.Location?.Name,
            DanceClassId = updated.DanceClassId,
            DanceClassName = updated.DanceClass?.Name,
            StartTime = updated.StartTime,
            Duration = updated.Duration,
            IsRecurring = updated.IsRecurring,
            RecurrencePattern = updated.RecurrencePattern,
            RecurrenceEndDate = updated.RecurrenceEndDate,
            IsCancelled = updated.IsCancelled,
            EffectiveFrom = updated.EffectiveFrom,
            EffectiveTo = updated.EffectiveTo,
            IsActive = updated.IsActive,
            CreatedAt = updated.CreatedAt,
            UpdatedAt = updated.UpdatedAt
        };

        return Ok(scheduleDto);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteSchedule(Guid id)
    {
        var result = await _scheduleService.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("weekly")]
    public async Task<ActionResult<WeeklyScheduleDto>> GetWeeklySchedule()
    {
        var schedules = await _scheduleService.GetAllAsync();
        var activeSchedules = schedules.Where(s => s.IsActive && !s.IsCancelled).ToList();

        // Load all dance classes to get the proper information
        var danceClasses = await _danceClassService.GetAllAsync();
        var danceClassDict = danceClasses.ToDictionary(dc => dc.Id, dc => dc);

        var weeklySchedule = new Dictionary<string, List<ScheduleSlotDto>>();
        var daysOfWeek = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

        // Initialize each day with empty lists
        foreach (var day in daysOfWeek)
        {
            weeklySchedule[day] = new List<ScheduleSlotDto>();
        }

        // Group schedules by day of week
        foreach (var schedule in activeSchedules)
        {
            var dayOfWeek = schedule.StartTime.DayOfWeek;
            var dayName = dayOfWeek switch
            {
                DayOfWeek.Monday => "Monday",
                DayOfWeek.Tuesday => "Tuesday", 
                DayOfWeek.Wednesday => "Wednesday",
                DayOfWeek.Thursday => "Thursday",
                DayOfWeek.Friday => "Friday",
                DayOfWeek.Saturday => "Saturday",
                DayOfWeek.Sunday => "Sunday",
                _ => "Monday"
            };

            var endTime = schedule.StartTime.Add(schedule.Duration);
            var timeSlot = $"{schedule.StartTime:HH:mm} - {endTime:HH:mm}";

            // Get the dance class information
            DanceClass? danceClass = null;
            danceClassDict.TryGetValue(schedule.DanceClassId, out danceClass);

            // Use dance class information for accurate data
            var danceName = danceClass?.Name ?? schedule.Name;
            var level = danceClass?.Level ?? "P1";
            var style = danceClass?.Style ?? "UNKNOWN";

            // Determine background color based on dance style
            var backgroundColor = GetBackgroundColorByStyle(style, danceName);

            // Format level properly
            var formattedLevel = FormatLevel(level);

            // Format dance name for display
            var displayStyle = FormatDanceStyle(danceName, style);

            var slot = new ScheduleSlotDto
            {
                Id = schedule.Id,
                TimeSlot = timeSlot,
                DanceName = danceName,
                Level = formattedLevel,
                Style = displayStyle,
                BackgroundColor = backgroundColor,
                EffectiveFrom = schedule.EffectiveFrom.ToString("dd.MM.yyyy"),
                IsCancelled = schedule.IsCancelled,
                IsActive = schedule.IsActive
            };

            weeklySchedule[dayName].Add(slot);
        }

        // Sort each day's schedules by start time
        foreach (var day in weeklySchedule.Keys)
        {
            weeklySchedule[day] = weeklySchedule[day]
                .OrderBy(s => DateTime.ParseExact(s.TimeSlot.Split(" - ")[0], "HH:mm", null))
                .ToList();
        }

        return Ok(new WeeklyScheduleDto { Schedule = weeklySchedule });
    }

    private static string GetBackgroundColorByStyle(string style, string danceName)
    {
        var upperName = danceName.ToUpper();
        
        // Check for specific styling classes first
        if (upperName.Contains("LADIES STYLING") || upperName.Contains("HIGH HEELS"))
            return "#E40046";
            
        // Then check by style
        return style.ToUpper() switch
        {
            "CUBANA" => "#B08A47",
            "SALSA" => "#333333", 
            "BACHATA" => "#166693",
            "RUEDA" => "#DFAF29",
            "ZOUK" => "#6A1B9A",
            "KIZOMBA" => "#007C5A",
            "STYLE" => "#E40046", // For styling classes
            _ => "#999999"
        };
    }

    private static string FormatLevel(string level)
    {
        if (string.IsNullOrEmpty(level))
            return "Level P1";
            
        if (level.ToUpper() == "OPEN")
            return "OPEN level";
            
        if (level.StartsWith("Level "))
            return level;
            
        return $"Level {level}";
    }

    private static string FormatDanceStyle(string danceName, string style)
    {
        var upperName = danceName.ToUpper();
        
        // Handle specific dance names
        if (upperName.Contains("SALSA LADIES STYLING"))
            return "SALSA LADIES STYLING";
        if (upperName.Contains("HIGH HEELS"))
            return "HIGH HEELS SEXY DANCE";
        if (upperName.Contains("BACHATA LADIES STYLING"))
            return "BACHATA LADIES STYLING";
        if (upperName.Contains("SALSA KUBAŃSKA"))
            return "SALSA KUBAŃSKA";
        if (upperName.Contains("SALSA ON1"))
            return "SALSA on1";
        if (upperName.Contains("SALSA ON2"))
            return "SALSA on2";
        if (upperName.Contains("RUEDA"))
            return "RUEDA DE CASINO";
        if (upperName.Contains("KIZOMBA") && upperName.Contains("SEMBA"))
            return "KIZOMBA i SEMBA";
            
        // Default to the dance name
        return danceName;
    }
}
