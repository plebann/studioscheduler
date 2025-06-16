using Microsoft.EntityFrameworkCore;
using StudioScheduler.Core.Interfaces.Repositories;
using StudioScheduler.Core.Models;
using StudioScheduler.Infrastructure.Data;

namespace StudioScheduler.Infrastructure.Repositories;

public class LocationRepository : ILocationRepository
{
    private readonly ApplicationDbContext _context;

    public LocationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Location?> GetByIdAsync(Guid id)
    {
        return await _context.Locations
            .Include(l => l.Rooms)
            .Include(l => l.Schedules)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<IEnumerable<Location>> GetAllAsync()
    {
        return await _context.Locations
            .Include(l => l.Rooms)
            .Include(l => l.Schedules)
            .ToListAsync();
    }

    public async Task<Location> AddAsync(Location location)
    {
        _context.Locations.Add(location);
        await _context.SaveChangesAsync();
        return location;
    }

    public async Task<Location> UpdateAsync(Location location)
    {
        _context.Entry(location).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return location;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location == null)
            return false;

        _context.Locations.Remove(location);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Locations.AnyAsync(l => l.Id == id);
    }

    public async Task<IEnumerable<Room>> GetRoomsAsync(Guid locationId)
    {
        return await _context.Rooms
            .Where(r => r.LocationId == locationId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Schedule>> GetSchedulesAsync(Guid locationId)
    {
        return await _context.Schedules
            .Where(s => s.LocationId == locationId)
            .Include(s => s.DanceClass)
            .ToListAsync();
    }

    public async Task<Location?> GetByNameAsync(string name)
    {
        return await _context.Locations
            .FirstOrDefaultAsync(l => l.Name == name);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
