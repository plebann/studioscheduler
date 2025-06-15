using StudioScheduler.Core.Interfaces.Repositories;
using StudioScheduler.Core.Interfaces.Services;
using StudioScheduler.Core.Models;

namespace StudioScheduler.Infrastructure.Services;

public class LocationService : ILocationService
{
    private readonly ILocationRepository _repository;
    private readonly IRoomRepository _roomRepository;
    private readonly IScheduleRepository _scheduleRepository;

    public LocationService(
        ILocationRepository repository, 
        IRoomRepository roomRepository,
        IScheduleRepository scheduleRepository)
    {
        _repository = repository;
        _roomRepository = roomRepository;
        _scheduleRepository = scheduleRepository;
    }

    public async Task<Location?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Location>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Location> CreateAsync(Location location)
    {
        var created = await _repository.AddAsync(location);
        await _repository.SaveChangesAsync();
        return created;
    }

    public async Task<Location> UpdateAsync(Location location)
    {
        var updated = await _repository.UpdateAsync(location);
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

    public async Task<IEnumerable<Room>> GetLocationRoomsAsync(Guid locationId)
    {
        return await _roomRepository.GetByLocationIdAsync(locationId);
    }

    public async Task<IEnumerable<Schedule>> GetLocationSchedulesAsync(Guid locationId)
    {
        return await _scheduleRepository.GetByLocationAsync(locationId);
    }
}
