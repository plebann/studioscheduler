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
            DayOfWeek = s.DayOfWeek,
            StartTime = s.StartTime,
            Duration = s.Duration,
            IsActive = s.IsActive,
            IsCancelled = s.IsCancelled,
            Level = s.Level,
            InstructorName = s.Instructor != null ? $"{s.Instructor.FirstName} {s.Instructor.LastName}" : null,
            RoomName = s.Room?.Name,
            Capacity = s.Capacity
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
            DayOfWeek = schedule.DayOfWeek,
            StartTime = schedule.StartTime,
            Duration = schedule.Duration,
            IsRecurring = schedule.IsRecurring,
            IsCancelled = schedule.IsCancelled,
            EffectiveFrom = schedule.EffectiveFrom,
            EffectiveTo = schedule.EffectiveTo,
            IsActive = schedule.IsActive,
            CreatedAt = schedule.CreatedAt,
            UpdatedAt = schedule.UpdatedAt,
            Level = schedule.Level,
            InstructorId = schedule.InstructorId,
            InstructorName = schedule.Instructor != null ? $"{schedule.Instructor.FirstName} {schedule.Instructor.LastName}" : null,
            RoomId = schedule.RoomId,
            RoomName = schedule.Room?.Name,
            Capacity = schedule.Capacity
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
            DayOfWeek = createDto.DayOfWeek,
            StartTime = createDto.StartTime,
            Duration = createDto.Duration,
            IsRecurring = createDto.IsRecurring,
            EffectiveFrom = createDto.EffectiveFrom,
            EffectiveTo = createDto.EffectiveTo,
            IsActive = true,
            IsCancelled = false,
            Level = createDto.Level,
            InstructorId = createDto.InstructorId,
            RoomId = createDto.RoomId,
            Capacity = createDto.Capacity
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
            DayOfWeek = created.DayOfWeek,
            StartTime = created.StartTime,
            Duration = created.Duration,
            IsRecurring = created.IsRecurring,
            IsCancelled = created.IsCancelled,
            EffectiveFrom = created.EffectiveFrom,
            EffectiveTo = created.EffectiveTo,
            IsActive = created.IsActive,
            CreatedAt = created.CreatedAt,
            UpdatedAt = created.UpdatedAt,
            Level = created.Level,
            InstructorId = created.InstructorId,
            InstructorName = created.Instructor != null ? $"{created.Instructor.FirstName} {created.Instructor.LastName}" : null,
            RoomId = created.RoomId,
            RoomName = created.Room?.Name,
            Capacity = created.Capacity
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
            DayOfWeek = updateDto.DayOfWeek,
            StartTime = updateDto.StartTime,
            Duration = updateDto.Duration,
            IsRecurring = updateDto.IsRecurring,
            EffectiveFrom = updateDto.EffectiveFrom,
            EffectiveTo = updateDto.EffectiveTo,
            IsActive = updateDto.IsActive,
            IsCancelled = updateDto.IsCancelled,
            CreatedAt = existingSchedule.CreatedAt,
            UpdatedAt = DateTime.UtcNow,
            Level = updateDto.Level,
            InstructorId = updateDto.InstructorId,
            RoomId = updateDto.RoomId,
            Capacity = updateDto.Capacity
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
            DayOfWeek = updated.DayOfWeek,
            StartTime = updated.StartTime,
            Duration = updated.Duration,
            IsRecurring = updated.IsRecurring,
            IsCancelled = updated.IsCancelled,
            EffectiveFrom = updated.EffectiveFrom,
            EffectiveTo = updated.EffectiveTo,
            IsActive = updated.IsActive,
            CreatedAt = updated.CreatedAt,
            UpdatedAt = updated.UpdatedAt,
            Level = updated.Level,
            InstructorId = updated.InstructorId,
            InstructorName = updated.Instructor != null ? $"{updated.Instructor.FirstName} {updated.Instructor.LastName}" : null,
            RoomId = updated.RoomId,
            RoomName = updated.Room?.Name,
            Capacity = updated.Capacity
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
        var daysOfWeek = Enum.GetNames<DayOfWeek>().ToList();
        // Initialize each day with empty lists
        foreach (var day in daysOfWeek)
        {
            weeklySchedule[day] = [];
        }

        // Group schedules by day of week
        foreach (var schedule in activeSchedules)
        {
            var dayName = schedule.DayOfWeek.ToString();

            var endTime = schedule.StartTime.Add(TimeSpan.FromMinutes(schedule.Duration));
            var timeSlot = $"{schedule.StartTime:hh\\:mm} - {endTime:hh\\:mm}";

            // Get the dance class information
            danceClassDict.TryGetValue(schedule.DanceClassId, out DanceClass? danceClass);

            // Use dance class information for accurate data
            var danceName = danceClass?.Name ?? schedule.Name;
            var level = schedule.Level;
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
                .OrderBy(s => TimeSpan.ParseExact(s.TimeSlot.Split(" - ")[0], @"hh\:mm", null))
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
