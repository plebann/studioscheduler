using StudioScheduler.Core.Enums;

namespace StudioScheduler.Core.Models;

public class Reservation
{
    public Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public User? User { get; set; }
    public required Guid ScheduleId { get; set; }
    public Schedule? Schedule { get; set; }
    public required Guid PassId { get; set; }
    public Pass? Pass { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public bool HasAttended { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
