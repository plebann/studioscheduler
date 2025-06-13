using StudioScheduler.Core.Models;

namespace StudioScheduler.Core.Interfaces.Services;

public interface ILocationService
{
    Task<Location?> GetByIdAsync(Guid id);
    Task<IEnumerable<Location>> GetAllAsync();
    Task<Location> CreateAsync(Location location);
    Task<Location> UpdateAsync(Location location);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<IEnumerable<Room>> GetLocationRoomsAsync(Guid locationId);
    Task<IEnumerable<Schedule>> GetLocationSchedulesAsync(Guid locationId);
}
