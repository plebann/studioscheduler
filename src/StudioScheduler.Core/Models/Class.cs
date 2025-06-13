namespace StudioScheduler.Core.Models;

public class Class
{
    public Guid Id { get; set; }
    
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required int Capacity { get; set; }
    public required Guid InstructorId { get; set; }
    public User? Instructor { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public ICollection<Schedule> Schedules { get; init; } = new List<Schedule>();
}
