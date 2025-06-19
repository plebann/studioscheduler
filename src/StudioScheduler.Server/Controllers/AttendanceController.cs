using Microsoft.AspNetCore.Mvc;
using StudioScheduler.Core.Interfaces.Services;
using StudioScheduler.Core.Interfaces.Repositories;
using StudioScheduler.Shared.Dtos;

namespace StudioScheduler.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendanceController : ControllerBase
{
    private readonly IClassAttendanceService _classAttendanceService;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly ILogger<AttendanceController> _logger;

    public AttendanceController(
        IClassAttendanceService classAttendanceService,
        IEnrollmentRepository enrollmentRepository,
        IStudentRepository studentRepository,
        IAttendanceRepository attendanceRepository,
        ILogger<AttendanceController> logger)
    {
        _classAttendanceService = classAttendanceService;
        _enrollmentRepository = enrollmentRepository;
        _studentRepository = studentRepository;
        _attendanceRepository = attendanceRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get class schedule information
    /// </summary>
    /// <param name="scheduleId">The schedule ID of the class</param>
    /// <returns>Schedule data</returns>
    [HttpGet("class/{scheduleId}")]
    public async Task<ActionResult<ClassAttendanceDto>> GetClassSchedule(string scheduleId)
    {
        try
        {
            if (!Guid.TryParse(scheduleId, out var scheduleGuid))
            {
                return BadRequest("Invalid schedule ID format");
            }

            _logger.LogInformation("Getting class schedule for schedule {ScheduleId}", scheduleId);

            var schedule = await _classAttendanceService.GetClassAttendanceAsync(scheduleGuid);
            if (schedule == null)
            {
                _logger.LogWarning("Schedule not found for ID {ScheduleId}", scheduleId);
                return NotFound($"Schedule not found for ID {scheduleId}");
            }

            // Get enrolled students from repository data
            var enrollments = await _enrollmentRepository.GetByScheduleIdAsync(scheduleGuid);
            var enrolledStudents = new List<StudentAttendanceDto>();

            foreach (var enrollment in enrollments.Where(e => e.IsActive))
            {
                var student = await _studentRepository.GetByIdAsync(enrollment.StudentId);
                if (student == null) continue;

                // Get attendance history for this student and schedule
                var attendanceHistory = await _attendanceRepository.GetByStudentAndScheduleAsync(enrollment.StudentId, scheduleGuid);
                
                // Cache the current pass to avoid multiple computed property evaluations
                var currentPass = student.CurrentPass;
                
                // Check if pass is expired
                var isPassExpired = currentPass != null && currentPass.EndDate < DateTime.UtcNow;
                var hasActivePass = currentPass?.IsActive == true && !isPassExpired;
                
                // Check if student is marked present for today
                var today = DateTime.Today;
                var isMarkedPresentToday = attendanceHistory.Any(a => 
                    a.ClassDate.Date == today && a.WasPresent);
                
                // Map student to StudentAttendanceDto
                var studentDto = new StudentAttendanceDto
                {
                    StudentId = student.Id.ToString(),
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    IsMarkedPresentToday = isMarkedPresentToday,
                    CanAttendToday = hasActivePass,
                    AttendanceNote = isPassExpired ? "Pass expired" : 
                                   currentPass == null ? "No active pass" : null,
                    CurrentPass = currentPass != null ? new StudentPassDto
                    {
                        PassId = currentPass.Id.ToString(),
                        PassType = currentPass.Type.ToString(),
                        StartDate = currentPass.StartDate,
                        EndDate = currentPass.EndDate,
                        TotalClasses = currentPass.TotalClasses,
                        RemainingClasses = currentPass.CalculateRemainingClasses(attendanceHistory),
                        ClassesPerWeek = currentPass.ClassesPerWeek,
                        Price = 200.00m,
                        IsActive = currentPass.IsActive,
                        IsExpired = isPassExpired,
                        ClassesUsedForThisClass = attendanceHistory.Count(a => a.WasPresent),
                        MaxClassesForThisClassType = currentPass.TotalClasses
                    } : null,
                    AttendanceHistory = attendanceHistory.Select(a => new AttendanceRecordDto
                    {
                        ClassDate = a.ClassDate,
                        WeekOffset = (int)Math.Floor((DateTime.Now - a.ClassDate).TotalDays / 7),
                        WasPresent = a.WasPresent,
                        PassUsed = a.PassUsed?.ToString() ?? null,
                        PassClassNumber = a.PassClassNumber,
                        IsPassActive = a.Pass?.IsActive ?? false
                    }).ToList()
                };

                enrolledStudents.Add(studentDto);
            }

            var classAttendanceDto = new ClassAttendanceDto
            {
                ScheduleId = schedule.Id.ToString(),
                ClassName = schedule.DanceClass?.Name ?? schedule.Name,
                DayOfWeek = schedule.DayOfWeek.ToString(),
                StartTime = schedule.StartTime.ToString(@"hh\:mm"),
                Instructor = schedule.Instructor != null ? $"{schedule.Instructor.FirstName} {schedule.Instructor.LastName}" : "TBD",
                Level = schedule.Level,
                Style = schedule.DanceClass?.Style ?? "Unknown",
                EnrolledStudents = enrolledStudents
            };

            _logger.LogInformation("Successfully retrieved schedule {ScheduleId}", scheduleId);
            return Ok(classAttendanceDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting schedule for ID {ScheduleId}", scheduleId);
            return StatusCode(500, new { message = "An error occurred while retrieving schedule", error = ex.Message });
        }
    }

    /// <summary>
    /// Mark a student's attendance for a specific class
    /// </summary>
    /// <param name="request">Attendance marking request</param>
    /// <returns>Result of the attendance marking operation</returns>
    [HttpPost("mark")]
    public async Task<ActionResult<MarkAttendanceResponseDto>> MarkAttendance([FromBody] MarkAttendanceRequestDto request)
    {
        try
        {
            if (!Guid.TryParse(request.ScheduleId, out var scheduleGuid) ||
                !Guid.TryParse(request.StudentId, out var studentGuid))
            {
                return BadRequest("Invalid schedule or student ID format");
            }

            _logger.LogInformation("Marking attendance for student {StudentId} in schedule {ScheduleId} as {IsPresent}", 
                request.StudentId, request.ScheduleId, request.IsPresent ? "present" : "absent");

            var attendance = await _classAttendanceService.MarkAttendanceAsync(
                scheduleGuid, studentGuid, request.IsPresent, request.Notes);
            
            var response = new MarkAttendanceResponseDto
            {
                Success = true,
                Message = $"Attendance marked as {(request.IsPresent ? "present" : "absent")}",
                UpdatedStudent = null // Would need to map from domain model if needed
            };

            _logger.LogInformation("Successfully marked attendance for student {StudentId}", request.StudentId);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid request: {Message}", ex.Message);
            return BadRequest(new MarkAttendanceResponseDto 
            { 
                Success = false, 
                Message = ex.Message,
                UpdatedStudent = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking attendance for student {StudentId} in schedule {ScheduleId}", 
                request.StudentId, request.ScheduleId);
            return StatusCode(500, new MarkAttendanceResponseDto 
            { 
                Success = false, 
                Message = "An error occurred while marking attendance",
                UpdatedStudent = null
            });
        }
    }

    /// <summary>
    /// Search for students by name
    /// </summary>
    /// <param name="searchTerm">Search term (minimum 3 characters)</param>
    /// <returns>List of matching students</returns>
    [HttpGet("search")]
    public async Task<ActionResult> SearchStudents([FromQuery] string searchTerm)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 3)
            {
                return BadRequest("Search term must be at least 3 characters long");
            }

            _logger.LogInformation("Searching for students with term: {SearchTerm}", searchTerm);

            var students = await _classAttendanceService.SearchStudentsAsync(searchTerm);
            
            _logger.LogInformation("Found {StudentCount} students matching search term: {SearchTerm}", 
                students.Count(), searchTerm);

            return Ok(students);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for students with term: {SearchTerm}", searchTerm);
            return StatusCode(500, new { message = "An error occurred while searching for students", error = ex.Message });
        }
    }
}
