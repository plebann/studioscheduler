using Microsoft.AspNetCore.Mvc;
using StudioScheduler.Core.Interfaces.Services;
using StudioScheduler.Core.Models;
using StudioScheduler.Shared.Dtos;

namespace StudioScheduler.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClassesController : ControllerBase
{
    private readonly IDanceClassService _classService;

    public ClassesController(IDanceClassService classService)
    {
        _classService = classService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClassSummaryDto>>> GetClasses()
    {
        var classes = await _classService.GetAllAsync();
        var summaries = classes.Select(c => new ClassSummaryDto
        {
            Id = c.Id,
            Name = c.Name,
            InstructorName = c.Instructor != null ? $"{c.Instructor.FirstName} {c.Instructor.LastName}" : null,
            Capacity = c.Capacity,
            IsActive = c.IsActive
        });
        
        return Ok(summaries);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ClassDto>> GetClass(Guid id)
    {
        var danceClass = await _classService.GetByIdAsync(id);
        if (danceClass == null)
        {
            return NotFound();
        }

        var classDto = new ClassDto
        {
            Id = danceClass.Id,
            Name = danceClass.Name,
            Description = danceClass.Description,
            Capacity = danceClass.Capacity,
            InstructorId = danceClass.InstructorId,
            InstructorName = danceClass.Instructor != null ? $"{danceClass.Instructor.FirstName} {danceClass.Instructor.LastName}" : null,
            IsActive = danceClass.IsActive,
            CreatedAt = danceClass.CreatedAt,
            UpdatedAt = danceClass.UpdatedAt
        };

        return Ok(classDto);
    }

    [HttpPost]
    public async Task<ActionResult<ClassDto>> CreateClass(CreateClassDto createDto)
    {
        var danceClass = new DanceClass
        {
            Name = createDto.Name,
            Description = createDto.Description,
            Level = createDto.Level,
            Style = createDto.Style,
            Capacity = createDto.Capacity,
            InstructorId = createDto.InstructorId,
            RoomId = createDto.RoomId,
            IsActive = true
        };

        var created = await _classService.CreateAsync(danceClass);
        
        var classDto = new ClassDto
        {
            Id = created.Id,
            Name = created.Name,
            Description = created.Description,
            Capacity = created.Capacity,
            InstructorId = created.InstructorId,
            InstructorName = created.Instructor != null ? $"{created.Instructor.FirstName} {created.Instructor.LastName}" : null,
            IsActive = created.IsActive,
            CreatedAt = created.CreatedAt,
            UpdatedAt = created.UpdatedAt
        };

        return CreatedAtAction(nameof(GetClass), new { id = classDto.Id }, classDto);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ClassDto>> UpdateClass(Guid id, UpdateClassDto updateDto)
    {
        var existingClass = await _classService.GetByIdAsync(id);
        if (existingClass == null)
        {
            return NotFound();
        }

        var updatedClass = new DanceClass
        {
            Id = existingClass.Id,
            Name = updateDto.Name,
            Description = updateDto.Description,
            Level = updateDto.Level,
            Style = updateDto.Style,
            Capacity = updateDto.Capacity,
            InstructorId = updateDto.InstructorId,
            RoomId = updateDto.RoomId,
            IsActive = updateDto.IsActive,
            CreatedAt = existingClass.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        var updated = await _classService.UpdateAsync(updatedClass);

        var classDto = new ClassDto
        {
            Id = updated.Id,
            Name = updated.Name,
            Description = updated.Description,
            Capacity = updated.Capacity,
            InstructorId = updated.InstructorId,
            InstructorName = updated.Instructor != null ? $"{updated.Instructor.FirstName} {updated.Instructor.LastName}" : null,
            IsActive = updated.IsActive,
            CreatedAt = updated.CreatedAt,
            UpdatedAt = updated.UpdatedAt
        };

        return Ok(classDto);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteClass(Guid id)
    {
        var result = await _classService.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}
