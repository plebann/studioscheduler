using StudioScheduler.Core.Interfaces.Repositories;
using StudioScheduler.Core.Interfaces.Services;
using StudioScheduler.Core.Models;

namespace StudioScheduler.Core.Services;

public class LocationService : ILocationService
{
    private readonly ILocationRepository _locationRepository;

    public LocationService(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    public async Task<Location?> GetByIdAsync(Guid id)
    {
        return await _locationRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Location>> GetAllAsync()
    {
        return await _locationRepository.GetAllAsync();
    }

    public async Task<Location> CreateAsync(Location location)
    {
        var result = await _locationRepository.AddAsync(location);
        await _locationRepository.SaveChangesAsync();
        return result;
    }

    public async Task<Location> UpdateAsync(Location location)
    {
        var result = await _locationRepository.UpdateAsync(location);
        await _locationRepository.SaveChangesAsync();
        return result;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var result = await _locationRepository.DeleteAsync(id);
        if (result)
        {
            await _locationRepository.SaveChangesAsync();
        }
        return result;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _locationRepository.ExistsAsync(id);
    }

    public async Task<IEnumerable<Room>> GetLocationRoomsAsync(Guid locationId)
    {
        return await _locationRepository.GetRoomsAsync(locationId);
    }

    public async Task<IEnumerable<Schedule>> GetLocationSchedulesAsync(Guid locationId)
    {
        return await _locationRepository.GetSchedulesAsync(locationId);
    }
}
