using Microsoft.AspNetCore.Mvc;
using StudioScheduler.Core.Interfaces.Services;
using StudioScheduler.Core.Models;
using StudioScheduler.Shared.Dtos;

namespace StudioScheduler.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly ILocationService _locationService;

    public LocationsController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LocationSummaryDto>>> GetLocations()
    {
        var locations = await _locationService.GetAllAsync();
        var summaries = locations.Select(l => new LocationSummaryDto
        {
            Id = l.Id,
            Name = l.Name,
            Address = l.Address,
            Capacity = l.Capacity,
            IsActive = l.IsActive
        });
        
        return Ok(summaries);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LocationDto>> GetLocation(Guid id)
    {
        var location = await _locationService.GetByIdAsync(id);
        if (location == null)
        {
            return NotFound();
        }

        var locationDto = new LocationDto
        {
            Id = location.Id,
            Name = location.Name,
            Address = location.Address,
            Description = location.Description,
            Capacity = location.Capacity,
            OpeningTime = location.OpeningTime,
            ClosingTime = location.ClosingTime,
            IsActive = location.IsActive,
            CreatedAt = location.CreatedAt,
            UpdatedAt = location.UpdatedAt
        };

        return Ok(locationDto);
    }

    [HttpPost]
    public async Task<ActionResult<LocationDto>> CreateLocation(CreateLocationDto createDto)
    {
        var location = new Location
        {
            Name = createDto.Name,
            Address = createDto.Address,
            Description = createDto.Description ?? string.Empty,
            Capacity = createDto.Capacity,
            OpeningTime = createDto.OpeningTime,
            ClosingTime = createDto.ClosingTime,
            IsActive = true
        };

        var created = await _locationService.CreateAsync(location);
        
        var locationDto = new LocationDto
        {
            Id = created.Id,
            Name = created.Name,
            Address = created.Address,
            Description = created.Description,
            Capacity = created.Capacity,
            OpeningTime = created.OpeningTime,
            ClosingTime = created.ClosingTime,
            IsActive = created.IsActive,
            CreatedAt = created.CreatedAt,
            UpdatedAt = created.UpdatedAt
        };

        return CreatedAtAction(nameof(GetLocation), new { id = locationDto.Id }, locationDto);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<LocationDto>> UpdateLocation(Guid id, UpdateLocationDto updateDto)
    {
        var existingLocation = await _locationService.GetByIdAsync(id);
        if (existingLocation == null)
        {
            return NotFound();
        }

        var updatedLocation = new Location
        {
            Id = existingLocation.Id,
            Name = updateDto.Name,
            Address = updateDto.Address,
            Description = updateDto.Description ?? string.Empty,
            Capacity = updateDto.Capacity,
            OpeningTime = updateDto.OpeningTime,
            ClosingTime = updateDto.ClosingTime,
            IsActive = updateDto.IsActive,
            CreatedAt = existingLocation.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        var updated = await _locationService.UpdateAsync(updatedLocation);

        var locationDto = new LocationDto
        {
            Id = updated.Id,
            Name = updated.Name,
            Address = updated.Address,
            Description = updated.Description,
            Capacity = updated.Capacity,
            OpeningTime = updated.OpeningTime,
            ClosingTime = updated.ClosingTime,
            IsActive = updated.IsActive,
            CreatedAt = updated.CreatedAt,
            UpdatedAt = updated.UpdatedAt
        };

        return Ok(locationDto);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteLocation(Guid id)
    {
        var result = await _locationService.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("{id:guid}/rooms")]
    public async Task<ActionResult<IEnumerable<Room>>> GetLocationRooms(Guid id)
    {
        if (!await _locationService.ExistsAsync(id))
        {
            return NotFound();
        }

        var rooms = await _locationService.GetLocationRoomsAsync(id);
        return Ok(rooms);
    }

    [HttpGet("{id:guid}/schedules")]
    public async Task<ActionResult<IEnumerable<Schedule>>> GetLocationSchedules(Guid id)
    {
        if (!await _locationService.ExistsAsync(id))
        {
            return NotFound();
        }

        var schedules = await _locationService.GetLocationSchedulesAsync(id);
        return Ok(schedules);
    }
}
