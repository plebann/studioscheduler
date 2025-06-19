using Microsoft.AspNetCore.Mvc;
using StudioScheduler.Core.Interfaces.Repositories;
using StudioScheduler.Core.Models;
using StudioScheduler.Core.Enums;
using StudioScheduler.Shared.Dtos;

namespace StudioScheduler.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentRepository _studentRepository;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(IStudentRepository studentRepository, ILogger<StudentsController> logger)
    {
        _studentRepository = studentRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudents()
    {
        try
        {
            var students = await _studentRepository.GetAllAsync();
            var studentDtos = students.Select(MapToStudentDto);
            
            return Ok(studentDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving students");
            return StatusCode(500, "An error occurred while retrieving students");
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StudentDto>> GetStudent(Guid id)
    {
        try
        {
            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            var studentDto = MapToStudentDto(student);
            return Ok(studentDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student {StudentId}", id);
            return StatusCode(500, "An error occurred while retrieving the student");
        }
    }

    [HttpPost]
    public async Task<ActionResult<StudentDto>> CreateStudent(CreateStudentDto createDto)
    {
        try
        {
            // Check if email already exists
            var existingStudent = await _studentRepository.GetByEmailAsync(createDto.Email);
            if (existingStudent != null)
            {
                return BadRequest("A student with this email already exists");
            }

            // Simple password hash (for demo purposes - use proper hashing in production)
            var passwordHash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(createDto.Password + "salt"));

            var student = new Student
            {
                Id = Guid.NewGuid(),
                Email = createDto.Email,
                FirstName = createDto.FirstName,
                LastName = createDto.LastName,
                PasswordHash = passwordHash,
                Phone = createDto.Phone,
                Gender = createDto.Gender,
                DateOfBirth = createDto.DateOfBirth,
                Role = UserRole.Student,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _studentRepository.CreateAsync(student);
            var studentDto = MapToStudentDto(created);

            return CreatedAtAction(nameof(GetStudent), new { id = studentDto.Id }, studentDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating student");
            return StatusCode(500, "An error occurred while creating the student");
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<StudentDto>> UpdateStudent(Guid id, UpdateStudentDto updateDto)
    {
        try
        {
            var existingStudent = await _studentRepository.GetByIdAsync(id);
            if (existingStudent == null)
            {
                return NotFound();
            }

            // Update the existing student object in place
            existingStudent.FirstName = updateDto.FirstName;
            existingStudent.LastName = updateDto.LastName;
            existingStudent.Phone = updateDto.Phone;
            existingStudent.Gender = updateDto.Gender;
            existingStudent.DateOfBirth = updateDto.DateOfBirth;
            existingStudent.IsActive = updateDto.IsActive;
            existingStudent.UpdatedAt = DateTime.UtcNow;

            var updated = await _studentRepository.UpdateAsync(existingStudent);
            var studentDto = MapToStudentDto(updated);

            return Ok(studentDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating student {StudentId}", id);
            return StatusCode(500, "An error occurred while updating the student");
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteStudent(Guid id)
    {
        try
        {
            var result = await _studentRepository.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting student {StudentId}", id);
            return StatusCode(500, "An error occurred while deleting the student");
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<StudentSummaryDto>>> SearchStudents([FromQuery] string searchTerm)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2)
            {
                return BadRequest("Search term must be at least 2 characters long");
            }

            var students = await _studentRepository.GetAllAsync();
            var filteredStudents = students.Where(s => 
                s.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                s.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                s.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            );

            var studentSummaries = filteredStudents.Select(s => new StudentSummaryDto
            {
                Id = s.Id,
                Name = $"{s.FirstName} {s.LastName}",
                Email = s.Email,
                IsActive = s.IsActive
            });

            return Ok(studentSummaries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching students with term: {SearchTerm}", searchTerm);
            return StatusCode(500, "An error occurred while searching for students");
        }
    }

    private static StudentDto MapToStudentDto(Student student)
    {
        return new StudentDto
        {
            Id = student.Id,
            Name = $"{student.FirstName} {student.LastName}",
            Email = student.Email,
            Phone = student.Phone,
            JoinDate = student.CreatedAt,
            IsActive = student.IsActive,
            Role = student.Role
        };
    }
}
