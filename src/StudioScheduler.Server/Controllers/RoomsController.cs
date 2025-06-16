using Microsoft.AspNetCore.Mvc;
using StudioScheduler.Core.Interfaces.Services;
using StudioScheduler.Core.Models;
using StudioScheduler.Shared.Dtos;

namespace StudioScheduler.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomSummaryDto>>> GetRooms()
    {
        var rooms = await _roomService.GetAllAsync();
        var summaries = rooms.Select(r => new RoomSummaryDto
        {
            Id = r.Id,
            Name = r.Name,
            LocationName = r.Location?.Name,
            Capacity = r.Capacity
        });
        
        return Ok(summaries);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RoomDto>> GetRoom(Guid id)
    {
        var room = await _roomService.GetByIdAsync(id);
        if (room == null)
        {
            return NotFound();
        }

        var roomDto = new RoomDto
        {
            Id = room.Id,
            Name = room.Name,
            LocationId = room.LocationId,
            LocationName = room.Location?.Name,
            Capacity = room.Capacity,
            Description = room.Description,
            Equipment = room.Equipment,
            CreatedAt = room.CreatedAt,
            UpdatedAt = room.UpdatedAt
        };

        return Ok(roomDto);
    }

    [HttpPost]
    public async Task<ActionResult<RoomDto>> CreateRoom(CreateRoomDto createDto)
    {
        var room = new Room
        {
            Name = createDto.Name,
            LocationId = createDto.LocationId,
            Capacity = createDto.Capacity,
            Description = createDto.Description ?? string.Empty,
            Equipment = createDto.Equipment?.ToList() ?? new List<string>()
        };

        var created = await _roomService.CreateAsync(room);
        
        var roomDto = new RoomDto
        {
            Id = created.Id,
            Name = created.Name,
            LocationId = created.LocationId,
            LocationName = created.Location?.Name,
            Capacity = created.Capacity,
            Description = created.Description,
            Equipment = created.Equipment,
            CreatedAt = created.CreatedAt,
            UpdatedAt = created.UpdatedAt
        };

        return CreatedAtAction(nameof(GetRoom), new { id = roomDto.Id }, roomDto);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<RoomDto>> UpdateRoom(Guid id, UpdateRoomDto updateDto)
    {
        var existingRoom = await _roomService.GetByIdAsync(id);
        if (existingRoom == null)
        {
            return NotFound();
        }

        var updatedRoom = new Room
        {
            Id = existingRoom.Id,
            Name = updateDto.Name,
            LocationId = existingRoom.LocationId,
            Capacity = updateDto.Capacity,
            Description = updateDto.Description ?? string.Empty,
            Equipment = updateDto.Equipment?.ToList() ?? new List<string>(),
            CreatedAt = existingRoom.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        var updated = await _roomService.UpdateAsync(updatedRoom);

        var roomDto = new RoomDto
        {
            Id = updated.Id,
            Name = updated.Name,
            LocationId = updated.LocationId,
            LocationName = updated.Location?.Name,
            Capacity = updated.Capacity,
            Description = updated.Description,
            Equipment = updated.Equipment,
            CreatedAt = updated.CreatedAt,
            UpdatedAt = updated.UpdatedAt
        };

        return Ok(roomDto);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteRoom(Guid id)
    {
        var result = await _roomService.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}
