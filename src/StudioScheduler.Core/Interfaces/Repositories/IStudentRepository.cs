using StudioScheduler.Core.Models;

namespace StudioScheduler.Core.Interfaces.Repositories;

public interface IStudentRepository
{
    Task<IEnumerable<Student>> GetAllAsync();
    Task<Student?> GetByIdAsync(Guid id);
    Task<IEnumerable<Student>> GetByScheduleIdAsync(Guid scheduleId);
    Task<Student> CreateAsync(Student student);
    Task<Student> UpdateAsync(Student student);
    Task<bool> DeleteAsync(Guid id);
}
