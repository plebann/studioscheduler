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

    // CUSTOM EXCEPTIONS FOR DUPLICATE PREVENTION
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

    public async Task<Enrollment?> GetByIdAsync(Guid id)
    {
        return await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Schedule)
            .ThenInclude(s => s.DanceClass)
            .Include(e => e.Schedule)
            .ThenInclude(s => s.Room)
            .ThenInclude(r => r.Location)
            .Include(e => e.Schedule)
            .ThenInclude(s => s.Instructor)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Enrollment>> GetAllAsync()
    {
        return await _context.Enrollments
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

    public async Task<Enrollment> CreateAsync(Enrollment enrollment)
    {
        // DUPLICATE PREVENTION: Check for existing active enrollment
        var existingEnrollment = await GetActiveByStudentAndScheduleAsync(
            enrollment.StudentId, enrollment.ScheduleId);
        
        if (existingEnrollment != null)
        {
            throw new DuplicateEnrollmentException(
                $"Active enrollment already exists for Student {enrollment.StudentId} and Schedule {enrollment.ScheduleId}",
                enrollment.StudentId, enrollment.ScheduleId);
        }
        
        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();
        return enrollment;
    }

    public async Task<IEnumerable<Enrollment>> CreateBatchAsync(IEnumerable<Enrollment> enrollments)
    {
        // DUPLICATE PREVENTION: Validate each enrollment before batch creation
        foreach (var enrollment in enrollments)
        {
            var existingEnrollment = await GetActiveByStudentAndScheduleAsync(
                enrollment.StudentId, enrollment.ScheduleId);
            
            if (existingEnrollment != null)
            {
                throw new DuplicateEnrollmentException(
                    $"Active enrollment already exists for Student {enrollment.StudentId} and Schedule {enrollment.ScheduleId}",
                    enrollment.StudentId, enrollment.ScheduleId);
            }
        }
        
        _context.Enrollments.AddRange(enrollments);
        await _context.SaveChangesAsync();
        return enrollments;
    }

    // NEW METHOD: Get active enrollment for student and schedule
    public async Task<Enrollment?> GetActiveByStudentAndScheduleAsync(Guid studentId, Guid scheduleId)
    {
        return await _context.Enrollments
            .FirstOrDefaultAsync(e => e.StudentId == studentId 
                                && e.ScheduleId == scheduleId 
                                && e.IsActive);
    }

    // NEW METHOD: Create or reactivate enrollment (single enrollment logic)
    public async Task<Enrollment> CreateOrReactivateAsync(Enrollment newEnrollment)
    {
        // Check for ANY existing enrollment (active or inactive)
        var existingEnrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e => e.StudentId == newEnrollment.StudentId 
                                && e.ScheduleId == newEnrollment.ScheduleId);
        
        if (existingEnrollment != null)
        {
            // REACTIVATION LOGIC: Update existing enrollment
            existingEnrollment.IsActive = true;
            existingEnrollment.EnrolledDate = newEnrollment.EnrolledDate;
            existingEnrollment.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return existingEnrollment;
        }
        else
        {
            // CREATE NEW: No existing enrollment found
            return await CreateAsync(newEnrollment);
        }
    }

    // NEW METHOD: Check if active enrollment exists
    public async Task<bool> HasActiveEnrollmentAsync(Guid studentId, Guid scheduleId)
    {
        return await _context.Enrollments
            .AnyAsync(e => e.StudentId == studentId 
                        && e.ScheduleId == scheduleId 
                        && e.IsActive);
    }

    // NEW METHOD: Get distinct student count for schedule (proper capacity calculation)
    public async Task<int> GetDistinctStudentCountAsync(Guid scheduleId)
    {
        return await _context.Enrollments
            .Where(e => e.ScheduleId == scheduleId && e.IsActive)
            .Select(e => e.StudentId)
            .Distinct()
            .CountAsync();
    }

    // NEW METHOD: Get all active enrollments for schedule (distinct students only)
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
            .Where(e => e.Schedule.DanceClassId == classId)
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
