using StudioScheduler.Core.Models;

namespace StudioScheduler.Core.Interfaces.Repositories;

public interface IEnrollmentRepository
{
    Task<IEnumerable<Enrollment>> GetAllAsync();
    Task<Enrollment?> GetByIdAsync(Guid id);
    Task<IEnumerable<Enrollment>> GetByStudentIdAsync(Guid studentId);
    Task<IEnumerable<Enrollment>> GetByScheduleIdAsync(Guid scheduleId);
    Task<Enrollment> CreateAsync(Enrollment enrollment);
    Task<IEnumerable<Enrollment>> CreateBatchAsync(IEnumerable<Enrollment> enrollments);
    Task<Enrollment> UpdateAsync(Enrollment enrollment);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<Enrollment>> GetByClassIdAsync(Guid classId);
    Task<Enrollment?> GetByStudentAndScheduleAsync(Guid studentId, Guid scheduleId);
    Task<bool> IsStudentEnrolledAsync(Guid studentId, Guid scheduleId);
    Task<int> GetEnrollmentCountAsync(Guid scheduleId);
    Task<bool> ExistsAsync(Guid id);
    Task SaveChangesAsync();
    Task<Enrollment?> GetActiveByStudentAndScheduleAsync(Guid studentId, Guid scheduleId);
    Task<Enrollment> CreateOrReactivateAsync(Enrollment newEnrollment);
    Task<bool> HasActiveEnrollmentAsync(Guid studentId, Guid scheduleId);
    Task<int> GetDistinctStudentCountAsync(Guid scheduleId);
    Task<IEnumerable<Enrollment>> GetActiveEnrollmentsByScheduleAsync(Guid scheduleId);
}
