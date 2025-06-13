using StudioScheduler.Core.Interfaces.Repositories;
using StudioScheduler.Core.Interfaces.Services;
using StudioScheduler.Core.Models;

namespace StudioScheduler.Core.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly ILocationRepository _locationRepository;

    public RoomService(IRoomRepository roomRepository, ILocationRepository locationRepository)
    {
        _roomRepository = roomRepository;
        _locationRepository = locationRepository;
    }

    public async Task<Room?> GetByIdAsync(Guid id)
    {
        return await _roomRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Room>> GetAllAsync()
    {
        return await _roomRepository.GetAllAsync();
    }

    public async Task<Room> CreateAsync(Room room)
    {
        if (!await _locationRepository.ExistsAsync(room.LocationId))
        {
            throw new ArgumentException("Location does not exist", nameof(room.LocationId));
        }

        var existingRoom = await _roomRepository.GetByLocationAndNameAsync(room.LocationId, room.Name);
        if (existingRoom != null)
        {
            throw new InvalidOperationException($"Room with name '{room.Name}' already exists in this location");
        }

        var result = await _roomRepository.AddAsync(room);
        await _roomRepository.SaveChangesAsync();
        return result;
    }

    public async Task<Room> UpdateAsync(Room room)
    {
        if (!await _locationRepository.ExistsAsync(room.LocationId))
        {
            throw new ArgumentException("Location does not exist", nameof(room.LocationId));
        }

        var existingRoom = await _roomRepository.GetByLocationAndNameAsync(room.LocationId, room.Name);
        if (existingRoom != null && existingRoom.Id != room.Id)
        {
            throw new InvalidOperationException($"Room with name '{room.Name}' already exists in this location");
        }

        var result = await _roomRepository.UpdateAsync(room);
        await _roomRepository.SaveChangesAsync();
        return result;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var result = await _roomRepository.DeleteAsync(id);
        if (result)
        {
            await _roomRepository.SaveChangesAsync();
        }
        return result;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _roomRepository.ExistsAsync(id);
    }

    public async Task<bool> IsRoomAvailableAsync(Guid roomId, DateTime startTime, TimeSpan duration)
    {
        return await _roomRepository.IsAvailableAsync(roomId, startTime, duration);
    }

    public async Task<IEnumerable<DanceClass>> GetRoomClassesAsync(Guid roomId)
    {
        return await _roomRepository.GetClassesAsync(roomId);
    }

    public async Task<Room?> GetRoomByLocationAndNameAsync(Guid locationId, string name)
    {
        return await _roomRepository.GetByLocationAndNameAsync(locationId, name);
    }
}
