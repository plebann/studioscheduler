namespace StudioScheduler.Core.Models;

public class Schedule
{
    public Guid Id { get; set; } = Guid.NewGuid();
    private string _name = null!;
    
    public required string Name 
    { 
        get => _name;
        set => _name = value ?? throw new ArgumentNullException(nameof(Name));
    }
    
    public required Guid LocationId { get; set; }
    public Location? Location { get; set; }
    public required DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;
    
    public required Guid DanceClassId { get; set; }
    public DanceClass? DanceClass { get; set; }
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
