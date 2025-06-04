namespace StudioScheduler.Core.Models;

public class Schedule
{
    public Guid Id { get; set; }
    public required Guid ClassId { get; set; }
    public Class? Class { get; set; }
    public required DateTime StartTime { get; set; }
    public required TimeSpan Duration { get; set; }
    public bool IsRecurring { get; set; }
    public string? RecurrencePattern { get; set; }
    public DateTime? RecurrenceEndDate { get; set; }
    public bool IsCancelled { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public ICollection<Reservation> Reservations { get; init; } = new List<Reservation>();
}
