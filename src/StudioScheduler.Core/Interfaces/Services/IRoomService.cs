using StudioScheduler.Core.Models;

namespace StudioScheduler.Core.Interfaces.Services;

public interface IRoomService
{
    Task<Room?> GetByIdAsync(Guid id);
    Task<IEnumerable<Room>> GetAllAsync();
    Task<Room> CreateAsync(Room room);
    Task<Room> UpdateAsync(Room room);
    Task<bool> DeleteAsync(Guid id);
}
