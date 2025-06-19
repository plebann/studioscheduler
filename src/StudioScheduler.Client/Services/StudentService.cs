using System.Net.Http.Json;
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

public class StudentService : IStudentService
{
    private readonly HttpClient _httpClient;

    public StudentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<StudentDto>> GetAllStudentsAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<StudentDto>>("api/students");
            return response ?? new List<StudentDto>();
        }
        catch (HttpRequestException)
        {
            // Log error in real implementation
            return new List<StudentDto>();
        }
    }

    public async Task<StudentDto?> GetStudentByIdAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<StudentDto>($"api/students/{id}");
        }
        catch (HttpRequestException)
        {
            // Log error in real implementation
            return null;
        }
    }

    public async Task<StudentDto> CreateStudentAsync(CreateStudentDto student)
    {
        var response = await _httpClient.PostAsJsonAsync("api/students", student);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<StudentDto>() 
               ?? throw new InvalidOperationException("Failed to deserialize created student");
    }

    public async Task<StudentDto> UpdateStudentAsync(Guid id, UpdateStudentDto student)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/students/{id}", student);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<StudentDto>() 
               ?? throw new InvalidOperationException("Failed to deserialize updated student");
    }

    public async Task<bool> DeleteStudentAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/students/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }

    public async Task<List<StudentSummaryDto>> SearchStudentsAsync(string searchTerm)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<StudentSummaryDto>>($"api/students/search?searchTerm={Uri.EscapeDataString(searchTerm)}");
            return response ?? new List<StudentSummaryDto>();
        }
        catch (HttpRequestException)
        {
            // Log error in real implementation
            return new List<StudentSummaryDto>();
        }
    }
}
