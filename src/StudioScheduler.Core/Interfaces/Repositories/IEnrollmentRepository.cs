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
}
