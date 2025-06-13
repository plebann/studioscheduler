using StudioScheduler.Core.Models;

namespace StudioScheduler.Core.Interfaces.Repositories;

public interface IRoomRepository
{
    Task<Room?> GetByIdAsync(Guid id);
    Task<IEnumerable<Room>> GetAllAsync();
    Task<Room> AddAsync(Room room);
    Task<Room> UpdateAsync(Room room);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<IEnumerable<Room>> GetByLocationIdAsync(Guid locationId);
    Task<Room?> GetByLocationAndNameAsync(Guid locationId, string name);
    Task<IEnumerable<DanceClass>> GetClassesAsync(Guid roomId);
    Task<bool> IsAvailableAsync(Guid roomId, DateTime startTime, TimeSpan duration);
    Task SaveChangesAsync();
}
