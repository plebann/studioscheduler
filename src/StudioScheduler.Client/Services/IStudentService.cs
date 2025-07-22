using StudioScheduler.Shared.Dtos;

namespace StudioScheduler.Client.Services;

public interface IStudentService
{
    Task<List<StudentDto>> GetAllStudentsAsync();
    Task<StudentDto?> GetStudentByIdAsync(Guid id);
    Task<StudentDto> CreateStudentAsync(CreateStudentDto student);
    Task<StudentDto> UpdateStudentAsync(Guid id, UpdateStudentDto student);
    Task<bool> DeleteStudentAsync(Guid id);
    Task<List<StudentSummaryDto>> SearchStudentsAsync(string searchTerm);
}
