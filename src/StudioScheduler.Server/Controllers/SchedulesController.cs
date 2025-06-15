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

    public SchedulesController(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
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
}
