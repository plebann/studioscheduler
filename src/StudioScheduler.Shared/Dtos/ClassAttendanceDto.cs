using System.ComponentModel.DataAnnotations;

namespace StudioScheduler.Shared.Dtos;

public class ClassAttendanceDto
{
    public required string ScheduleId { get; set; }
    public required string ClassName { get; set; }
    public required string DayOfWeek { get; set; }
    public required string StartTime { get; set; }
    public required string Instructor { get; set; }
    public required string Level { get; set; }
    public required string Style { get; set; }
    public required List<StudentAttendanceDto> EnrolledStudents { get; set; } = new();
}

public class StudentAttendanceDto
{
    public required string StudentId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    
    // Current Pass Information
    public StudentPassDto? CurrentPass { get; set; }
    
    // Attendance History (last 3 weeks + today)
    public List<AttendanceRecordDto> AttendanceHistory { get; set; } = new();
    
    // Today's attendance
    public bool IsMarkedPresentToday { get; set; }
    public bool CanAttendToday { get; set; }
    public string? AttendanceNote { get; set; }
}

public class StudentPassDto
{
    public required string PassId { get; set; }
    public required string PassType { get; set; }
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
    public required int TotalClasses { get; set; }
    public required int RemainingClasses { get; set; }
    public required int ClassesPerWeek { get; set; }
    public required decimal Price { get; set; }
    public required bool IsActive { get; set; }
    public required bool IsExpired { get; set; }
    
    // For this specific class
    public int ClassesUsedForThisClass { get; set; }
    public int MaxClassesForThisClassType { get; set; }
    
    public string PassStatusDisplay => IsExpired ? "EXPIRED" : IsActive ? "ACTIVE" : "INACTIVE";
    public string PassUsageDisplay => ClassesPerWeek > 0 
        ? $"{ClassesUsedForThisClass}/{MaxClassesForThisClassType}" 
        : $"{TotalClasses - RemainingClasses}/{TotalClasses}";
}

public class AttendanceRecordDto
{
    public required DateTime ClassDate { get; set; }
    public required int WeekOffset { get; set; } // -2, -1, 0 (today)
    public required bool WasPresent { get; set; }
    public string? PassUsed { get; set; }
    public int PassClassNumber { get; set; }
    public bool IsPassActive { get; set; }
    
    public string WeekLabel => WeekOffset switch
    {
        -2 => "2wk ago",
        -1 => "Last wk",
        0 => "Today",
        _ => $"{Math.Abs(WeekOffset)}wk ago"
    };
}

public class MarkAttendanceRequestDto
{
    [Required]
    public required string ScheduleId { get; set; }
    
    [Required]
    public required string StudentId { get; set; }
    
    [Required]
    public required bool IsPresent { get; set; }
    
    public string? Notes { get; set; }
}

public class MarkAttendanceResponseDto
{
    public required bool Success { get; set; }
    public required string Message { get; set; }
    public StudentAttendanceDto? UpdatedStudent { get; set; }
}
