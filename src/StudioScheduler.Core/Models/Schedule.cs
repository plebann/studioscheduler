namespace StudioScheduler.Core.Models;

public class Schedule
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public required string Name { get; init; }
    
    public required Guid LocationId { get; set; }
    public Location? Location { get; set; }
    public required DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;
    
    public required Guid DanceClassId { get; set; }
    public DanceClass? DanceClass { get; set; }
    public required DayOfWeek DayOfWeek { get; set; }
    public required TimeSpan StartTime { get; set; }
    public required int Duration { get; set; }
    public bool IsRecurring { get; set; }
    public DateTime? RecurrenceEndDate { get; set; }
    public bool IsCancelled { get; set; }
    
    public required string Level { get; set; }
    public Guid? InstructorId { get; set; }
    public User? Instructor { get; set; }
    public Guid? RoomId { get; set; }
    public Room? Room { get; set; }
    public required int Capacity { get; set; }
    
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    public ICollection<Enrollment> Enrollments { get; init; } = new List<Enrollment>();
}
