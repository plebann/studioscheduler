using Microsoft.EntityFrameworkCore;
using StudioScheduler.Core.Interfaces.Repositories;
using StudioScheduler.Core.Models;
using StudioScheduler.Infrastructure.Data;

namespace StudioScheduler.Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly ApplicationDbContext _context;

    public StudentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Student?> GetByIdAsync(Guid id)
    {
        return await _context.Students
            .Include(s => s.Passes)
            .Include(s => s.Enrollments)
            .ThenInclude(e => e.Schedule)
            .ThenInclude(s => s.DanceClass)
            .Include(s => s.AttendanceRecords)
            .ThenInclude(a => a.Schedule)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Student>> GetAllAsync()
    {
        return await _context.Students
            .Include(s => s.Passes)
            .Include(s => s.Enrollments)
            .ThenInclude(e => e.Schedule)
            .ThenInclude(s => s.DanceClass)
            .Include(s => s.AttendanceRecords)
            .ThenInclude(a => a.Schedule)
            .ToListAsync();
    }

    public async Task<Student> CreateAsync(Student student)
    {
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<Student> UpdateAsync(Student student)
    {
        _context.Entry(student).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null)
            return false;

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Students.AnyAsync(s => s.Id == id);
    }

    public async Task<Student?> GetByEmailAsync(string email)
    {
        return await _context.Students
            .Include(s => s.Passes)
            .Include(s => s.Enrollments)
            .ThenInclude(e => e.Schedule)
            .ThenInclude(s => s.DanceClass)
            .Include(s => s.AttendanceRecords)
            .ThenInclude(a => a.Schedule)
            .FirstOrDefaultAsync(s => s.Email == email);
    }

    public async Task<IEnumerable<Student>> GetByScheduleIdAsync(Guid scheduleId)
    {
        return await _context.Students
            .Where(s => s.Enrollments.Any(e => e.ScheduleId == scheduleId))
            .Include(s => s.Passes)
            .Include(s => s.Enrollments)
            .ThenInclude(e => e.Schedule)
            .ThenInclude(s => s.DanceClass)
            .Include(s => s.AttendanceRecords)
            .ThenInclude(a => a.Schedule)
            .ToListAsync();
    }

    public async Task<IEnumerable<Student>> GetByClassIdAsync(Guid classId)
    {
        return await _context.Students
            .Where(s => s.Enrollments.Any(e => e.Schedule.DanceClassId == classId))
            .Include(s => s.Passes)
            .Include(s => s.Enrollments)
            .ThenInclude(e => e.Schedule)
            .ThenInclude(s => s.DanceClass)
            .Include(s => s.AttendanceRecords)
            .ThenInclude(a => a.Schedule)
            .ToListAsync();
    }

    public async Task<IEnumerable<Enrollment>> GetEnrollmentsAsync(Guid studentId)
    {
        return await _context.Enrollments
            .Where(e => e.StudentId == studentId)
            .Include(e => e.Schedule)
            .ThenInclude(s => s.DanceClass)
            .Include(e => e.Schedule)
            .ThenInclude(s => s.Room)
            .ThenInclude(r => r.Location)
            .Include(e => e.Student)
            .ToListAsync();
    }

    public async Task<IEnumerable<Attendance>> GetAttendanceRecordsAsync(Guid studentId)
    {
        return await _context.Attendances
            .Where(a => a.StudentId == studentId)
            .Include(a => a.Schedule)
            .ThenInclude(s => s.DanceClass)
            .Include(a => a.Schedule)
            .ThenInclude(s => s.Room)
            .ThenInclude(r => r.Location)
            .Include(a => a.Student)
            .OrderByDescending(a => a.ClassDate)
            .ToListAsync();
    }

    public async Task<bool> IsEnrolledInClassAsync(Guid studentId, Guid scheduleId)
    {
        return await _context.Enrollments
            .AnyAsync(e => e.StudentId == studentId && e.ScheduleId == scheduleId);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
