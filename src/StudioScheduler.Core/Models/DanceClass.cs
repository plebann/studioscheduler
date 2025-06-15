namespace StudioScheduler.Core.Models;

public class DanceClass
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Level { get; init; }
    public required string Style { get; init; }
    public required int Capacity { get; set; }
    public Guid? InstructorId { get; set; }
    public User? Instructor { get; set; }
    public Guid? RoomId { get; set; }
    public Room? Room { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public ICollection<Schedule> Schedules { get; init; } = new List<Schedule>();
}
