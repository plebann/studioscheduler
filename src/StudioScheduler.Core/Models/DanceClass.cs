namespace StudioScheduler.Core.Models;

public class DanceClass
{
    private bool _isActive = true;
    
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Style { get; init; }
    
    public bool IsActive 
    { 
        get => _isActive;
        set
        {
            if (_isActive != value)
            {
                _isActive = value;
                UpdatedAt = DateTime.UtcNow;
            }
        }
    }
    
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public ICollection<Schedule> Schedules { get; init; } = new List<Schedule>();
}
