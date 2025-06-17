namespace StudioScheduler.Core.Models;

public class Room
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string Name { get; init; }
    public string Description { get; init; } = string.Empty;

    public required int Capacity { get; set; }
    public required Guid LocationId { get; set; }
    public Location? Location { get; set; }
    public bool IsActive { get; set; } = true;
    public List<string> Equipment { get; set; } = new();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Schedule> Schedules { get; init; } = new List<Schedule>();
}
