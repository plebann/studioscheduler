using Microsoft.EntityFrameworkCore;
using StudioScheduler.Core.Interfaces.Repositories;
using StudioScheduler.Core.Models;
using StudioScheduler.Infrastructure.Data;

namespace StudioScheduler.Infrastructure.Repositories;

public class DanceClassRepository : IDanceClassRepository
{
    private readonly ApplicationDbContext _context;

    public DanceClassRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DanceClass?> GetByIdAsync(Guid id)
    {
        return await _context.DanceClasses
            .Include(c => c.Room)
            .ThenInclude(r => r.Location)
            .Include(c => c.Instructor)
            .Include(c => c.Schedules)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<DanceClass>> GetAllAsync()
    {
        return await _context.DanceClasses
            .Include(c => c.Room)
            .ThenInclude(r => r.Location)
            .Include(c => c.Instructor)
            .Include(c => c.Schedules)
            .ToListAsync();
    }

    public async Task<DanceClass> AddAsync(DanceClass danceClass)
    {
        _context.DanceClasses.Add(danceClass);
        await _context.SaveChangesAsync();
        return danceClass;
    }

    public async Task<DanceClass> UpdateAsync(DanceClass danceClass)
    {
        _context.Entry(danceClass).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return danceClass;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var danceClass = await _context.DanceClasses.FindAsync(id);
        if (danceClass == null)
            return false;

        _context.DanceClasses.Remove(danceClass);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.DanceClasses.AnyAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<DanceClass>> GetByStyleAsync(string style)
    {
        return await _context.DanceClasses
            .Where(c => c.Style == style)
            .Include(c => c.Room)
            .ThenInclude(r => r.Location)
            .Include(c => c.Instructor)
            .Include(c => c.Schedules)
            .ToListAsync();
    }

    public async Task<IEnumerable<DanceClass>> GetByLevelAsync(string level)
    {
        return await _context.DanceClasses
            .Where(c => c.Level == level)
            .Include(c => c.Room)
            .ThenInclude(r => r.Location)
            .Include(c => c.Instructor)
            .Include(c => c.Schedules)
            .ToListAsync();
    }

    public async Task<IEnumerable<DanceClass>> GetByInstructorAsync(Guid instructorId)
    {
        return await _context.DanceClasses
            .Where(c => c.InstructorId == instructorId)
            .Include(c => c.Room)
            .ThenInclude(r => r.Location)
            .Include(c => c.Instructor)
            .Include(c => c.Schedules)
            .ToListAsync();
    }

    public async Task<IEnumerable<DanceClass>> GetByRoomAsync(Guid roomId)
    {
        return await _context.DanceClasses
            .Where(c => c.RoomId == roomId)
            .Include(c => c.Room)
            .ThenInclude(r => r.Location)
            .Include(c => c.Instructor)
            .Include(c => c.Schedules)
            .ToListAsync();
    }

    public async Task<IEnumerable<DanceClass>> GetByLocationAsync(Guid locationId)
    {
        return await _context.DanceClasses
            .Where(c => c.Room.LocationId == locationId)
            .Include(c => c.Room)
            .ThenInclude(r => r.Location)
            .Include(c => c.Instructor)
            .Include(c => c.Schedules)
            .ToListAsync();
    }

    public async Task<DanceClass?> GetByNameAsync(string name)
    {
        return await _context.DanceClasses
            .FirstOrDefaultAsync(c => c.Name == name);
    }

    public async Task<IEnumerable<Schedule>> GetSchedulesAsync(Guid classId)
    {
        return await _context.Schedules
            .Where(s => s.DanceClassId == classId)
            .Include(s => s.Location)
            .Include(s => s.DanceClass)
            .ToListAsync();
    }

    public async Task<int> GetCurrentEnrollmentAsync(Guid classId)
    {
        return await _context.Enrollments
            .Where(e => e.Schedule.DanceClassId == classId)
            .CountAsync();
    }

    public async Task<bool> IsInstructorAvailableAsync(Guid instructorId, DateTime startTime, TimeSpan duration)
    {
        var endTime = startTime.Add(duration);
        
        // Check if there are any overlapping schedules for this instructor
        var overlappingSchedules = await _context.Schedules
            .Where(s => s.DanceClass != null && s.DanceClass.InstructorId == instructorId)
            .Where(s => s.StartTime < endTime && s.StartTime.Add(s.Duration) > startTime)
            .AnyAsync();

        return !overlappingSchedules;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
