namespace StudioScheduler.Core.Models;

public class DanceClass
{
    public Guid Id { get; set; } = Guid.NewGuid();
    private string _name = null!;
    private string _description = null!;
    
    public required string Name 
    { 
        get => _name;
        set => _name = value ?? throw new ArgumentNullException(nameof(Name));
    }
    
    public required string Description 
    { 
        get => _description;
        set => _description = value ?? throw new ArgumentNullException(nameof(Description));
    }

    public required string Level { get; set; }
    public required string Style { get; set; }
    public required int Capacity { get; set; }
    public required Guid InstructorId { get; set; }
    public User? Instructor { get; set; }
    public required Guid RoomId { get; set; }
    public Room? Room { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public ICollection<Schedule> Schedules { get; init; } = new List<Schedule>();
}
