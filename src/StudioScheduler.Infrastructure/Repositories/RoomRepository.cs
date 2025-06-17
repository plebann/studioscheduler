using Microsoft.EntityFrameworkCore;
using StudioScheduler.Core.Interfaces.Repositories;
using StudioScheduler.Core.Models;
using StudioScheduler.Infrastructure.Data;

namespace StudioScheduler.Infrastructure.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly ApplicationDbContext _context;

    public RoomRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Room?> GetByIdAsync(Guid id)
    {
        return await _context.Rooms
            .Include(r => r.Location)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Room>> GetAllAsync()
    {
        return await _context.Rooms
            .Include(r => r.Location)
            .ToListAsync();
    }

    public async Task<Room> AddAsync(Room room)
    {
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();
        return room;
    }

    public async Task<Room> UpdateAsync(Room room)
    {
        _context.Entry(room).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return room;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
            return false;

        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Rooms.AnyAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Room>> GetByLocationIdAsync(Guid locationId)
    {
        return await _context.Rooms
            .Where(r => r.LocationId == locationId)
            .Include(r => r.Location)
            .ToListAsync();
    }

    public async Task<Room?> GetByLocationAndNameAsync(Guid locationId, string name)
    {
        return await _context.Rooms
            .FirstOrDefaultAsync(r => r.Name == name && r.LocationId == locationId);
    }

    public async Task<IEnumerable<Schedule>> GetSchedulesAsync(Guid roomId)
    {
        return await _context.Schedules
            .Where(s => s.RoomId == roomId)
            .Include(s => s.Room)
            .Include(s => s.Instructor)
            .Include(s => s.DanceClass)
            .ToListAsync();
    }

    public async Task<bool> IsAvailableAsync(Guid roomId, DateTime startTime, TimeSpan duration)
    {
        var endTime = startTime.Add(duration);
        
        // Check if there are any overlapping schedules for this room
        var overlappingSchedules = await _context.Schedules
            .Where(s => s.RoomId == roomId)
            .Where(s => s.StartTime < endTime && s.StartTime.Add(s.Duration) > startTime)
            .AnyAsync();

        return !overlappingSchedules;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
