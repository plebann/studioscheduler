namespace StudioScheduler.Core.Models;

public class Enrollment
{
    public Guid Id { get; set; }
    public required Guid StudentId { get; set; }
    public Student? Student { get; set; }
    public required Guid ScheduleId { get; set; }
    public Schedule? Schedule { get; set; }
    public required DateTime EnrolledDate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
