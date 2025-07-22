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

    public class DuplicateEnrollmentException : Exception
    {
        public Guid StudentId { get; }
        public Guid ScheduleId { get; }
        
        public DuplicateEnrollmentException(string message, Guid studentId, Guid scheduleId) 
            : base(message)
        {
            StudentId = studentId;
            ScheduleId = scheduleId;
        }
    }

    public async Task<Enrollment> CreateAsync(Enrollment enrollment)
    {
        // DUPLICATE PREVENTION: Check for existing active enrollment
        var existingEnrollment = await HasActiveEnrollmentAsync(
            enrollment.StudentId, enrollment.ScheduleId);

        if (existingEnrollment)
        {
            throw new DuplicateEnrollmentException(
                $"Active enrollment already exists for Student {enrollment.StudentId} and Schedule {enrollment.ScheduleId}",
                enrollment.StudentId, enrollment.ScheduleId);
        }
        
        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();
        return enrollment;
    }

    public async Task<Enrollment> CreateOrReactivateAsync(Enrollment newEnrollment)
    {
        Enrollment? existingEnrollment = await GetExistingEnrollment(newEnrollment.StudentId, newEnrollment.ScheduleId);

        if (existingEnrollment is null)
        {
            return await CreateAsync(newEnrollment);
        }
        else
        {
            existingEnrollment.IsActive = true;
            existingEnrollment.EnrolledDate = newEnrollment.EnrolledDate;
            existingEnrollment.UpdatedAt = DateTime.UtcNow;

            return await UpdateAsync(existingEnrollment);
        }
    }

    private async Task<Enrollment?> GetExistingEnrollment(Guid studentId, Guid scheduleId)
    {
        // Check for ANY existing enrollment (active or inactive)
        return await _context.Enrollments
            .FirstOrDefaultAsync(e => e.StudentId == studentId
                                && e.ScheduleId == scheduleId);
    }

    public async Task<bool> HasActiveEnrollmentAsync(Guid studentId, Guid scheduleId)
    {
        return await _context.Enrollments
            .AnyAsync(e => e.StudentId == studentId 
                        && e.ScheduleId == scheduleId 
                        && e.IsActive);
    }

    // Get all active enrollments for schedule (distinct students only)
    public async Task<IEnumerable<Enrollment>> GetActiveEnrollmentsByScheduleAsync(Guid scheduleId)
    {
        return await _context.Enrollments
            .Where(e => e.ScheduleId == scheduleId && e.IsActive)
            .Include(e => e.Student)
            .Include(e => e.Schedule)
            .ThenInclude(s => s.DanceClass)
            .Include(e => e.Schedule)
            .ThenInclude(s => s.Room)
            .ThenInclude(r => r.Location)
            .Include(e => e.Schedule)
            .ThenInclude(s => s.Instructor)
            .ToListAsync();
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
            .Include(e => e.Schedule)
            .ThenInclude(s => s.Room)
            .ThenInclude(r => r.Location)
            .Include(e => e.Schedule)
            .ThenInclude(s => s.Instructor)
            .ToListAsync();
    }

    public async Task<IEnumerable<Enrollment>> GetByScheduleIdAsync(Guid scheduleId)
    {
        return await _context.Enrollments
            .Where(e => e.ScheduleId == scheduleId)
            .Include(e => e.Student)
            .Include(e => e.Schedule)
            .ThenInclude(s => s.DanceClass)
            .Include(e => e.Schedule)
            .ThenInclude(s => s.Room)
            .ThenInclude(r => r.Location)
            .Include(e => e.Schedule)
            .ThenInclude(s => s.Instructor)
            .ToListAsync();
    }

    public async Task<IEnumerable<Enrollment>> GetByClassIdAsync(Guid classId)
    {
        return await _context.Enrollments
            .Where(e => e.Schedule != null && e.Schedule.DanceClassId == classId)
            .Include(e => e.Student)
            .Include(e => e.Schedule)
            .ThenInclude(s => s.DanceClass)
            .Include(e => e.Schedule)
            .ThenInclude(s => s.Room)
            .ThenInclude(r => r.Location)
            .Include(e => e.Schedule)
            .ThenInclude(s => s.Instructor)
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
