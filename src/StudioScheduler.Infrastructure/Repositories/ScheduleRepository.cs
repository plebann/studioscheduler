using Microsoft.EntityFrameworkCore;
using StudioScheduler.Core.Interfaces.Repositories;
using StudioScheduler.Core.Models;
using StudioScheduler.Infrastructure.Data;

namespace StudioScheduler.Infrastructure.Repositories;

public class ScheduleRepository : IScheduleRepository
{
    private readonly ApplicationDbContext _context;

    public ScheduleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Schedule?> GetByIdAsync(Guid id)
    {
        return await _context.Schedules
            .Include(s => s.Location)
            .Include(s => s.DanceClass)
            .ThenInclude(c => c.Room)
            .Include(s => s.DanceClass)
            .ThenInclude(c => c.Instructor)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Schedule>> GetAllAsync()
    {
        return await _context.Schedules
            .Include(s => s.Location)
            .Include(s => s.DanceClass)
            .ThenInclude(c => c.Room)
            .Include(s => s.DanceClass)
            .ThenInclude(c => c.Instructor)
            .ToListAsync();
    }

    public async Task<Schedule> AddAsync(Schedule schedule)
    {
        _context.Schedules.Add(schedule);
        await _context.SaveChangesAsync();
        return schedule;
    }

    public async Task<Schedule> UpdateAsync(Schedule schedule)
    {
        _context.Entry(schedule).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return schedule;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var schedule = await _context.Schedules.FindAsync(id);
        if (schedule == null)
            return false;

        _context.Schedules.Remove(schedule);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Schedules.AnyAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Schedule>> GetByLocationAsync(Guid locationId)
    {
        return await _context.Schedules
            .Where(s => s.LocationId == locationId)
            .Include(s => s.Location)
            .Include(s => s.DanceClass)
            .ThenInclude(c => c.Room)
            .Include(s => s.DanceClass)
            .ThenInclude(c => c.Instructor)
            .ToListAsync();
    }

    public async Task<IEnumerable<Schedule>> GetByDanceClassAsync(Guid danceClassId)
    {
        return await _context.Schedules
            .Where(s => s.DanceClassId == danceClassId)
            .Include(s => s.Location)
            .Include(s => s.DanceClass)
            .ThenInclude(c => c.Room)
            .Include(s => s.DanceClass)
            .ThenInclude(c => c.Instructor)
            .ToListAsync();
    }

    public async Task<IEnumerable<Schedule>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Schedules
            .Where(s => s.StartTime >= startDate && s.StartTime <= endDate)
            .Include(s => s.Location)
            .Include(s => s.DanceClass)
            .ThenInclude(c => c.Room)
            .Include(s => s.DanceClass)
            .ThenInclude(c => c.Instructor)
            .ToListAsync();
    }


    public async Task<IEnumerable<Schedule>> GetActiveSchedulesAsync()
    {
        return await _context.Schedules
            .Where(s => s.IsActive)
            .Include(s => s.Location)
            .Include(s => s.DanceClass)
            .ThenInclude(c => c.Room)
            .Include(s => s.DanceClass)
            .ThenInclude(c => c.Instructor)
            .ToListAsync();
    }

    public async Task<IEnumerable<Schedule>> GetByInstructorAsync(Guid instructorId)
    {
        return await _context.Schedules
            .Where(s => s.DanceClass != null && s.DanceClass.InstructorId == instructorId)
            .Include(s => s.Location)
            .Include(s => s.DanceClass)
            .ThenInclude(c => c.Room)
            .Include(s => s.DanceClass)
            .ThenInclude(c => c.Instructor)
            .ToListAsync();
    }

    public async Task<bool> IsTimeSlotAvailableAsync(Guid roomId, DateTime startTime, TimeSpan duration)
    {
        var endTime = startTime.Add(duration);
        
        var hasConflict = await _context.Schedules
            .Where(s => s.DanceClass != null && s.DanceClass.RoomId == roomId)
            .Where(s => s.StartTime < endTime && s.StartTime.Add(s.Duration) > startTime)
            .AnyAsync();

        return !hasConflict;
    }

    public async Task<int> GetAvailableSpotsAsync(Guid scheduleId)
    {
        var schedule = await _context.Schedules
            .Include(s => s.DanceClass)
            .ThenInclude(c => c.Room)
            .FirstOrDefaultAsync(s => s.Id == scheduleId);

        if (schedule?.DanceClass?.Room == null)
            return 0;

        var enrollmentCount = await _context.Enrollments
            .CountAsync(e => e.ScheduleId == scheduleId);

        return Math.Max(0, schedule.DanceClass.Room.Capacity - enrollmentCount);
    }

    public async Task<bool> HasScheduleConflictAsync(Guid roomId, DateTime startTime, TimeSpan duration, Guid? excludeScheduleId = null)
    {
        var endTime = startTime.Add(duration);
        
        var query = _context.Schedules
            .Where(s => s.DanceClass != null && s.DanceClass.RoomId == roomId)
            .Where(s => s.StartTime < endTime && s.StartTime.Add(s.Duration) > startTime);

        if (excludeScheduleId.HasValue)
        {
            query = query.Where(s => s.Id != excludeScheduleId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<bool> CancelScheduleAsync(Guid scheduleId)
    {
        var schedule = await _context.Schedules.FindAsync(scheduleId);
        if (schedule == null)
            return false;

        schedule.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
