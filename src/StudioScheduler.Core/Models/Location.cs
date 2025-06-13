namespace StudioScheduler.Core.Models;

public class Location
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string Name { get; init; }
    public required string Address { get; init; }
    public string Description { get; init; } = string.Empty;

    public required int Capacity { get; set; }
    public required TimeSpan OpeningTime { get; set; }
    public required TimeSpan ClosingTime { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Room> Rooms { get; init; } = new List<Room>();
    public ICollection<Schedule> Schedules { get; init; } = new List<Schedule>();
}
