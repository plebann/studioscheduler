using StudioScheduler.Core.Enums;

namespace StudioScheduler.Core.Models;

public class Pass
{
    public Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public User? User { get; set; }
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
    public required PassType Type { get; set; } = PassType.SingleClass;
    public required int ClassesPerWeek { get; set; }
    public required int RemainingClasses { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public ICollection<Reservation> Reservations { get; init; } = new List<Reservation>();
}
