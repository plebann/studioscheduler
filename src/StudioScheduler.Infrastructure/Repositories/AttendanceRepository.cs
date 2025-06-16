using Microsoft.EntityFrameworkCore;
using StudioScheduler.Core.Interfaces.Repositories;
using StudioScheduler.Core.Models;
using StudioScheduler.Infrastructure.Data;

namespace StudioScheduler.Infrastructure.Repositories;

public class AttendanceRepository : IAttendanceRepository
{
    private readonly ApplicationDbContext _context;

    public AttendanceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Attendance?> GetByIdAsync(Guid id)
    {
        return await _context.Attendances
            .Include(a => a.Student)
            .Include(a => a.Schedule)
            .ThenInclude(s => s.DanceClass)
            .ThenInclude(c => c.Room)
            .ThenInclude(r => r.Location)
            .Include(a => a.Schedule)
            .ThenInclude(s => s.DanceClass)
            .ThenInclude(c => c.Instructor)
            .Include(a => a.Pass)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Attendance>> GetAllAsync()
    {
        return await _context.Attendances
            .Include(a => a.Student)
            .Include(a => a.Schedule)
            .ThenInclude(s => s.DanceClass)
            .ThenInclude(c => c.Room)
            .ThenInclude(r => r.Location)
            .Include(a => a.Schedule)
            .ThenInclude(s => s.DanceClass)
            .ThenInclude(c => c.Instructor)
            .Include(a => a.Pass)
            .OrderByDescending(a => a.ClassDate)
            .ToListAsync();
    }

    public async Task<Attendance> CreateAsync(Attendance attendance)
    {
        _context.Attendances.Add(attendance);
        await _context.SaveChangesAsync();
        return attendance;
    }

    public async Task<Attendance> UpdateAsync(Attendance attendance)
    {
        _context.Entry(attendance).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return attendance;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var attendance = await _context.Attendances.FindAsync(id);
        if (attendance == null)
            return false;

        _context.Attendances.Remove(attendance);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Attendances.AnyAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Attendance>> GetByStudentIdAsync(Guid studentId)
    {
        return await _context.Attendances
            .Where(a => a.StudentId == studentId)
            .Include(a => a.Student)
            .Include(a => a.Schedule)
            .ThenInclude(s => s.DanceClass)
            .ThenInclude(c => c.Room)
            .ThenInclude(r => r.Location)
            .Include(a => a.Schedule)
            .ThenInclude(s => s.DanceClass)
            .ThenInclude(c => c.Instructor)
            .Include(a => a.Pass)
            .OrderByDescending(a => a.ClassDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Attendance>> GetByScheduleIdAsync(Guid scheduleId)
    {
        return await _context.Attendances
            .Where(a => a.ScheduleId == scheduleId)
            .Include(a => a.Student)
            .Include(a => a.Schedule)
            .ThenInclude(s => s.DanceClass)
            .ThenInclude(c => c.Room)
            .ThenInclude(r => r.Location)
            .Include(a => a.Schedule)
            .ThenInclude(s => s.DanceClass)
            .ThenInclude(c => c.Instructor)
            .Include(a => a.Pass)
            .OrderByDescending(a => a.ClassDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Attendance>> GetByClassIdAsync(Guid classId)
    {
        return await _context.Attendances
            .Where(a => a.Schedule.DanceClassId == classId)
            .Include(a => a.Student)
            .Include(a => a.Schedule)
            .ThenInclude(s => s.DanceClass)
            .ThenInclude(c => c.Room)
            .ThenInclude(r => r.Location)
            .Include(a => a.Schedule)
            .ThenInclude(s => s.DanceClass)
            .ThenInclude(c => c.Instructor)
            .Include(a => a.Pass)
            .OrderByDescending(a => a.ClassDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Attendance>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Attendances
            .Where(a => a.ClassDate >= startDate && a.ClassDate <= endDate)
            .Include(a => a.Student)
            .Include(a => a.Schedule)
            .ThenInclude(s => s.DanceClass)
            .ThenInclude(c => c.Room)
            .ThenInclude(r => r.Location)
            .Include(a => a.Schedule)
            .ThenInclude(s => s.DanceClass)
            .ThenInclude(c => c.Instructor)
            .Include(a => a.Pass)
            .OrderByDescending(a => a.ClassDate)
            .ToListAsync();
    }

    public async Task<Attendance?> GetByStudentAndScheduleAndDateAsync(Guid studentId, Guid scheduleId, DateTime date)
    {
        return await _context.Attendances
            .Include(a => a.Student)
            .Include(a => a.Schedule)
            .ThenInclude(s => s.DanceClass)
            .Include(a => a.Pass)
            .FirstOrDefaultAsync(a => a.StudentId == studentId && 
                                   a.ScheduleId == scheduleId && 
                                   a.ClassDate.Date == date.Date);
    }

    public async Task<bool> HasAttendedAsync(Guid studentId, Guid scheduleId, DateTime date)
    {
        return await _context.Attendances
            .AnyAsync(a => a.StudentId == studentId && 
                          a.ScheduleId == scheduleId && 
                          a.ClassDate.Date == date.Date);
    }

    public async Task<int> GetAttendanceCountAsync(Guid scheduleId, DateTime date)
    {
        return await _context.Attendances
            .CountAsync(a => a.ScheduleId == scheduleId && a.ClassDate.Date == date.Date);
    }

    public async Task<IEnumerable<Attendance>> GetByStudentAndScheduleAsync(Guid studentId, Guid scheduleId)
    {
        return await _context.Attendances
            .Where(a => a.StudentId == studentId && a.ScheduleId == scheduleId)
            .Include(a => a.Student)
            .Include(a => a.Schedule)
            .ThenInclude(s => s.DanceClass)
            .ThenInclude(c => c.Room)
            .ThenInclude(r => r.Location)
            .Include(a => a.Schedule)
            .ThenInclude(s => s.DanceClass)
            .ThenInclude(c => c.Instructor)
            .Include(a => a.Pass)
            .OrderByDescending(a => a.ClassDate)
            .ToListAsync();
    }

    public async Task<Attendance?> GetByStudentScheduleAndDateAsync(Guid studentId, Guid scheduleId, DateTime classDate)
    {
        return await _context.Attendances
            .Include(a => a.Student)
            .Include(a => a.Schedule)
            .ThenInclude(s => s.DanceClass)
            .Include(a => a.Pass)
            .FirstOrDefaultAsync(a => a.StudentId == studentId && 
                                   a.ScheduleId == scheduleId && 
                                   a.ClassDate.Date == classDate.Date);
    }

    public async Task<IEnumerable<Attendance>> GetByPassIdAsync(Guid passId)
    {
        return await _context.Attendances
            .Where(a => a.PassUsed == passId)
            .Include(a => a.Student)
            .Include(a => a.Schedule)
            .ThenInclude(s => s.DanceClass)
            .ThenInclude(c => c.Room)
            .ThenInclude(r => r.Location)
            .Include(a => a.Schedule)
            .ThenInclude(s => s.DanceClass)
            .ThenInclude(c => c.Instructor)
            .Include(a => a.Pass)
            .OrderByDescending(a => a.ClassDate)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
