using Microsoft.EntityFrameworkCore;
using StudioScheduler.Core.Interfaces.Repositories;
using StudioScheduler.Core.Models;
using StudioScheduler.Infrastructure.Data;

namespace StudioScheduler.Infrastructure.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly ApplicationDbContext _context;

    public EnrollmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Enrollment?> GetByIdAsync(Guid id)
    {
        return await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Schedule)
            .ThenInclude(s => s.DanceClass)
            .ThenInclude(c => c.Room)
            .ThenInclude(r => r.Location)
            .Include(e => e.Schedule)
            .ThenInclude(s => s.DanceClass)
            .ThenInclude(c => c.Instructor)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Enrollment>> GetAllAsync()
    {
        return await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Schedule)
            .ThenInclude(s => s.DanceClass)
            .ThenInclude(c => c.Room)
            .ThenInclude(r => r.Location)
            .Include(e => e.Schedule)
            .ThenInclude(s => s.DanceClass)
            .ThenInclude(c => c.Instructor)
            .ToListAsync();
    }

    public async Task<Enrollment> CreateAsync(Enrollment enrollment)
    {
        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();
        return enrollment;
    }

    public async Task<Enrollment> UpdateAsync(Enrollment enrollment)
    {
        _context.Entry(enrollment).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return enrollment;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var enrollment = await _context.Enrollments.FindAsync(id);
        if (enrollment == null)
            return false;

        _context.Enrollments.Remove(enrollment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Enrollments.AnyAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Enrollment>> GetByStudentIdAsync(Guid studentId)
    {
        return await _context.Enrollments
            .Where(e => e.StudentId == studentId)
            .Include(e => e.Student)
            .Include(e => e.Schedule)
            .ThenInclude(s => s.DanceClass)
            .ThenInclude(c => c.Room)
            .ThenInclude(r => r.Location)
            .Include(e => e.Schedule)
            .ThenInclude(s => s.DanceClass)
            .ThenInclude(c => c.Instructor)
            .ToListAsync();
    }

    public async Task<IEnumerable<Enrollment>> GetByScheduleIdAsync(Guid scheduleId)
    {
        return await _context.Enrollments
            .Where(e => e.ScheduleId == scheduleId)
            .Include(e => e.Student)
            .Include(e => e.Schedule)
            .ThenInclude(s => s.DanceClass)
            .ThenInclude(c => c.Room)
            .ThenInclude(r => r.Location)
            .Include(e => e.Schedule)
            .ThenInclude(s => s.DanceClass)
            .ThenInclude(c => c.Instructor)
            .ToListAsync();
    }

    public async Task<IEnumerable<Enrollment>> GetByClassIdAsync(Guid classId)
    {
        return await _context.Enrollments
            .Where(e => e.Schedule.DanceClassId == classId)
            .Include(e => e.Student)
            .Include(e => e.Schedule)
            .ThenInclude(s => s.DanceClass)
            .ThenInclude(c => c.Room)
            .ThenInclude(r => r.Location)
            .Include(e => e.Schedule)
            .ThenInclude(s => s.DanceClass)
            .ThenInclude(c => c.Instructor)
            .ToListAsync();
    }

    public async Task<Enrollment?> GetByStudentAndScheduleAsync(Guid studentId, Guid scheduleId)
    {
        return await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Schedule)
            .ThenInclude(s => s.DanceClass)
            .FirstOrDefaultAsync(e => e.StudentId == studentId && e.ScheduleId == scheduleId);
    }

    public async Task<bool> IsStudentEnrolledAsync(Guid studentId, Guid scheduleId)
    {
        return await _context.Enrollments
            .AnyAsync(e => e.StudentId == studentId && e.ScheduleId == scheduleId);
    }

    public async Task<int> GetEnrollmentCountAsync(Guid scheduleId)
    {
        return await _context.Enrollments
            .CountAsync(e => e.ScheduleId == scheduleId);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
