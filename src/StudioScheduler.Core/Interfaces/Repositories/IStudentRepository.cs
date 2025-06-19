using StudioScheduler.Core.Models;

namespace StudioScheduler.Core.Interfaces.Repositories;

public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(Guid id);
    Task<IEnumerable<Student>> GetAllAsync();
    Task<Student> CreateAsync(Student student);
    Task<Student> UpdateAsync(Student student);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<Student?> GetByEmailAsync(string email);
    Task<IEnumerable<Student>> GetByScheduleIdAsync(Guid scheduleId);
    Task<IEnumerable<Student>> GetByClassIdAsync(Guid classId);
    Task<IEnumerable<Enrollment>> GetEnrollmentsAsync(Guid studentId);
    Task<IEnumerable<Attendance>> GetAttendanceRecordsAsync(Guid studentId);
    Task<bool> IsEnrolledInClassAsync(Guid studentId, Guid scheduleId);
    Task SaveChangesAsync();
}
