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
            .Include(r => r.Classes)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Room>> GetAllAsync()
    {
        return await _context.Rooms
            .Include(r => r.Location)
            .Include(r => r.Classes)
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
            .Include(r => r.Classes)
            .ToListAsync();
    }

    public async Task<Room?> GetByLocationAndNameAsync(Guid locationId, string name)
    {
        return await _context.Rooms
            .FirstOrDefaultAsync(r => r.Name == name && r.LocationId == locationId);
    }

    public async Task<IEnumerable<DanceClass>> GetClassesAsync(Guid roomId)
    {
        return await _context.DanceClasses
            .Where(c => c.RoomId == roomId)
            .Include(c => c.Room)
            .Include(c => c.Instructor)
            .ToListAsync();
    }

    public async Task<bool> IsAvailableAsync(Guid roomId, DateTime startTime, TimeSpan duration)
    {
        var endTime = startTime.Add(duration);
        
        // Check if there are any overlapping schedules for this room
        var overlappingSchedules = await _context.Schedules
            .Where(s => s.DanceClass != null && s.DanceClass.RoomId == roomId)
            .Where(s => s.StartTime < endTime && s.StartTime.Add(s.Duration) > startTime)
            .AnyAsync();

        return !overlappingSchedules;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
