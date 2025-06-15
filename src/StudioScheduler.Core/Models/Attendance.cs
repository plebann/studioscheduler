namespace StudioScheduler.Core.Models;

public class Attendance
{
    public Guid Id { get; set; }
    public required Guid StudentId { get; set; }
    public Student? Student { get; set; }
    public required Guid ScheduleId { get; set; }
    public Schedule? Schedule { get; set; }
    public required DateTime ClassDate { get; set; }
    public required bool WasPresent { get; set; }
    public Guid? PassUsed { get; set; }
    public Pass? Pass { get; set; }
    public int PassClassNumber { get; set; } // Which class number for this pass (1/4, 2/4, etc.)
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
