using StudioScheduler.Core.Enums;

namespace StudioScheduler.Core.Models;

public class Student : User
{
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public ICollection<Pass> Passes { get; init; } = new List<Pass>();
    public ICollection<Enrollment> Enrollments { get; init; } = new List<Enrollment>();
    public ICollection<Attendance> AttendanceRecords { get; init; } = new List<Attendance>();
    
    // Helper properties
    public Pass? CurrentPass
    {
        get
        {
            var today = DateTime.Today;
            return Passes
                .Where(p => p.IsActive && 
                           p.StartDate <= today && 
                           p.EndDate >= today)
                .OrderByDescending(p => p.StartDate)
                .FirstOrDefault();
        }
    }
}
