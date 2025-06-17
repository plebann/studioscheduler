using StudioScheduler.Core.Models;

namespace StudioScheduler.Core.Interfaces.Services;

public interface IRoomService
{
    Task<Room?> GetByIdAsync(Guid id);
    Task<IEnumerable<Room>> GetAllAsync();
    Task<Room> CreateAsync(Room room);
    Task<Room> UpdateAsync(Room room);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> IsRoomAvailableAsync(Guid roomId, DateTime startTime, TimeSpan duration);
    Task<IEnumerable<Schedule>> GetRoomSchedulesAsync(Guid roomId);
    Task<Room?> GetRoomByLocationAndNameAsync(Guid locationId, string name);
}
