namespace StudioScheduler.Core.Models;

public class Room
{
    public Guid Id { get; set; } = Guid.NewGuid();
    private string _name = null!;
    private string _description = null!;

    public required string Name
    {
        get => _name;
        set => _name = value ?? throw new ArgumentNullException(nameof(Name));
    }

    public string Description
    {
        get => _description;
        set => _description = value ?? throw new ArgumentNullException(nameof(Description));
    }

    public required int Capacity { get; set; }
    public required Guid LocationId { get; set; }
    public Location? Location { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<DanceClass> Classes { get; init; } = new List<DanceClass>();
}
