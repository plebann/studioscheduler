using Microsoft.AspNetCore.Mvc;
using StudioScheduler.Core.Interfaces.Services;
using StudioScheduler.Core.Interfaces.Repositories;
using StudioScheduler.Core.Models;
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
    private readonly IPassRepository _passRepository;
    private readonly ILogger<AttendanceController> _logger;

    public AttendanceController(
        IClassAttendanceService classAttendanceService,
        IEnrollmentRepository enrollmentRepository,
        IStudentRepository studentRepository,
        IAttendanceRepository attendanceRepository,
        IPassRepository passRepository,
        ILogger<AttendanceController> logger)
    {
        _classAttendanceService = classAttendanceService;
        _enrollmentRepository = enrollmentRepository;
        _studentRepository = studentRepository;
        _attendanceRepository = attendanceRepository;
        _passRepository = passRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get class schedule information with 4-week attendance/enrollment/cancellation window
    /// </summary>
    [HttpGet("class/{scheduleId}")]
    public async Task<ActionResult<ClassAttendanceDto>> GetClassSchedule(string scheduleId)
    {
        try
        {
            if (!Guid.TryParse(scheduleId, out var scheduleGuid))
                return BadRequest("Invalid schedule ID format");

            _logger.LogInformation("Getting class schedule for schedule {ScheduleId}", scheduleId);

            var schedule = await _classAttendanceService.GetClassAttendanceAsync(scheduleGuid);
            if (schedule == null)
                return NotFound($"Schedule not found for ID {scheduleId}");

            var enrollments = await _enrollmentRepository.GetByScheduleIdAsync(scheduleGuid);
            var enrolledStudents = new List<StudentAttendanceDto>();

            var today = DateTime.Today;
            // Use only enrollments with a valid pass (active, valid period)
            var validEnrollments = enrollments.Where(e => e.Pass != null && e.Pass.IsActive && e.Pass.StartDate <= today && e.Pass.EndDate >= today).ToList();
            var distinctEnrollments = validEnrollments
                .GroupBy(e => e.StudentId)
                .Select(g => g.OrderByDescending(e => e.EnrolledDate).First())
                .ToList();

            // Calculate the last 4 calendar weeks' class dates (including this week)
            var classDayOfWeek = schedule.DayOfWeek;
            var classDates = new List<DateTime>();
            var mostRecentClassDate = today.AddDays(-((7 + (int)today.DayOfWeek - (int)classDayOfWeek) % 7));
            for (int i = 0; i < 4; i++)
            {
                classDates.Add(mostRecentClassDate.AddDays(-7 * (3 - i)));
            }
            // Determine if the most recent class date is today
            bool mostRecentIsToday = mostRecentClassDate == today;

            foreach (var enrollment in distinctEnrollments)
            {
                var student = await _studentRepository.GetByIdAsync(enrollment.StudentId);
                if (student == null) continue;

                var attendanceHistory = await _attendanceRepository.GetByStudentAndScheduleAsync(enrollment.StudentId, scheduleGuid);
                var allPasses = await _passRepository.GetByUserIdAsync(enrollment.StudentId);

                var currentPass = student.CurrentPass;
                var futurePass = allPasses
                    .Where(p => p.IsActive && p.StartDate > today)
                    .OrderBy(p => p.StartDate)
                    .FirstOrDefault();
                var passToDisplay = currentPass ?? futurePass;
                var isPassExpired = currentPass != null && currentPass.EndDate < DateTime.UtcNow;
                var hasActivePass = currentPass?.IsActive == true && !isPassExpired;

                var attendanceRecords = new List<AttendanceRecordDto>();
                for (int i = 0; i < classDates.Count; i++)
                {
                    var classDate = classDates[i];
                    int weekOffset;
                    if (mostRecentIsToday)
                    {
                        weekOffset = 1 - (3 - i);
                    }
                    else
                    {
                        weekOffset = -(3 - i);
                    }

                    bool isEnrolled = enrollment.EnrolledDate.Date <= classDate.Date;

                    var allAttendanceForDate = await _attendanceRepository.GetByScheduleAndDateAsync(scheduleGuid, classDate);
                    bool isCanceled = allAttendanceForDate.Any(a => a.StudentId == null && a.IsCanceled);
                    if (!isCanceled)
                        isCanceled = allAttendanceForDate.Any(a => a.StudentId == enrollment.StudentId && a.IsCanceled);

                    var attendance = attendanceHistory.FirstOrDefault(a => a.ClassDate.Date == classDate.Date);
                    bool wasPresent = attendance?.WasPresent ?? false;

                    bool hasValidPass = allPasses.Any(p => p.StartDate <= classDate && p.EndDate >= classDate);

                    attendanceRecords.Add(new AttendanceRecordDto
                    {
                        ClassDate = classDate,
                        WeekOffset = weekOffset,
                        WasPresent = wasPresent,
                        IsEnrolled = isEnrolled,
                        IsCanceled = isCanceled,
                        PassUsed = attendance?.PassUsed?.ToString(),
                        PassClassNumber = attendance?.PassClassNumber ?? 0,
                        IsPassActive = attendance?.Pass?.IsActive ?? false,
                        IsVisible = hasValidPass
                    });
                }

                // Only set IsMarkedPresentToday and CanAttendToday if today is the scheduled class day
                bool isClassToday = today.DayOfWeek == classDayOfWeek;
                var isMarkedPresentToday = isClassToday && attendanceHistory.Any(a => a.ClassDate.Date == today && a.WasPresent);
                var canAttendToday = isClassToday && hasActivePass;

                // Determine attendance note based on pass status
                string? attendanceNote = null;
                if (currentPass != null && isPassExpired)
                    attendanceNote = "Pass expired";
                else if (currentPass == null && futurePass != null)
                    attendanceNote = "Not started yet";
                else if (currentPass == null && futurePass == null)
                    attendanceNote = "No active pass";

                var studentDto = new StudentAttendanceDto
                {
                    StudentId = student.Id.ToString(),
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    IsMarkedPresentToday = isMarkedPresentToday,
                    CanAttendToday = canAttendToday,
                    AttendanceNote = attendanceNote,
                    CurrentPass = passToDisplay != null ? new StudentPassDto
                    {
                        PassId = passToDisplay.Id.ToString(),
                        PassType = passToDisplay.Type.ToString(),
                        StartDate = passToDisplay.StartDate,
                        EndDate = passToDisplay.EndDate,
                        TotalClasses = passToDisplay.TotalClasses,
                        RemainingClasses = passToDisplay.CalculateRemainingClasses(attendanceHistory),
                        ClassesPerWeek = passToDisplay.ClassesPerWeek,
                        Price = 200.00m,
                        IsActive = passToDisplay.IsActive,
                        IsExpired = passToDisplay.EndDate < DateTime.UtcNow,
                        ClassesUsedForThisClass = attendanceHistory.Count(),
                        MaxClassesForThisClassType = passToDisplay.TotalClasses
                    } : null,
                    AttendanceHistory = attendanceRecords
                };

                enrolledStudents.Add(studentDto);
            }

            var classAttendanceDto = new ClassAttendanceDto
            {
                ScheduleId = schedule.Id.ToString(),
                ClassName = schedule.DanceClass?.Name ?? "Unknown",
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
}
