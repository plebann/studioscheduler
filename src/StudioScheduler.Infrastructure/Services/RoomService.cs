using StudioScheduler.Core.Interfaces.Repositories;
using StudioScheduler.Core.Interfaces.Services;
using StudioScheduler.Core.Models;

namespace StudioScheduler.Infrastructure.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _repository;

    public RoomService(IRoomRepository repository)
    {
        _repository = repository;
    }

    public async Task<Room?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Room>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Room> CreateAsync(Room room)
    {
        var created = await _repository.AddAsync(room);
        await _repository.SaveChangesAsync();
        return created;
    }

    public async Task<Room> UpdateAsync(Room room)
    {
        var updated = await _repository.UpdateAsync(room);
        await _repository.SaveChangesAsync();
        return updated;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var result = await _repository.DeleteAsync(id);
        if (result)
        {
            await _repository.SaveChangesAsync();
        }
        return result;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _repository.ExistsAsync(id);
    }

    public async Task<bool> IsRoomAvailableAsync(Guid roomId, DateTime startTime, TimeSpan duration)
    {
        return await _repository.IsAvailableAsync(roomId, startTime, duration);
    }

    public async Task<IEnumerable<DanceClass>> GetRoomClassesAsync(Guid roomId)
    {
        return await _repository.GetClassesAsync(roomId);
    }

    public async Task<Room?> GetRoomByLocationAndNameAsync(Guid locationId, string name)
    {
        return await _repository.GetByLocationAndNameAsync(locationId, name);
    }
}
