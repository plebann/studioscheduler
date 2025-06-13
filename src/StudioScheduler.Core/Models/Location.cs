namespace StudioScheduler.Core.Models;

public class Location
{
    public Guid Id { get; set; } = Guid.NewGuid();
    private string _name = null!;
    private string _address = null!;
    private string _description = null!;

    public required string Name
    {
        get => _name;
        set => _name = value ?? throw new ArgumentNullException(nameof(Name));
    }

    public required string Address
    {
        get => _address;
        set => _address = value ?? throw new ArgumentNullException(nameof(Address));
    }

    public string Description
    {
        get => _description;
        set => _description = value ?? throw new ArgumentNullException(nameof(Description));
    }

    public required int Capacity { get; set; }
    public required TimeSpan OpeningTime { get; set; }
    public required TimeSpan ClosingTime { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Room> Rooms { get; init; } = new List<Room>();
    public ICollection<Schedule> Schedules { get; init; } = new List<Schedule>();
}
