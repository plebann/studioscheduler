using StudioScheduler.Core.Models;

namespace StudioScheduler.Core.Interfaces.Repositories;

public interface ILocationRepository
{
    Task<Location?> GetByIdAsync(Guid id);
    Task<IEnumerable<Location>> GetAllAsync();
    Task<Location> AddAsync(Location location);
    Task<Location> UpdateAsync(Location location);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<IEnumerable<Room>> GetRoomsAsync(Guid locationId);
    Task<IEnumerable<Schedule>> GetSchedulesAsync(Guid locationId);
    Task<Location?> GetByNameAsync(string name);
    Task SaveChangesAsync();
}
