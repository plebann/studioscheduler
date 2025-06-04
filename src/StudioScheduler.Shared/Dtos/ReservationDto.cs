namespace StudioScheduler.Shared.Dtos;

public record ReservationDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public required string UserName { get; init; }
    public Guid ScheduleId { get; init; }
    public required string ClassName { get; init; }
    public required DateTime ClassStartTime { get; init; }
    public Guid PassId { get; init; }
    public required string PassType { get; init; }
    public DateTime CreatedAt { get; init; }
    public required string Status { get; init; }
    public DateTime? CancelledAt { get; init; }
    public string? CancellationReason { get; init; }
    public bool HasAttended { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record CreateReservationDto
{
    public required Guid ScheduleId { get; init; }
    public required Guid PassId { get; init; }
}

public record UpdateReservationStatusDto
{
    public required string Status { get; init; }
    public string? CancellationReason { get; init; }
}

public record UpdateReservationAttendanceDto
{
    public required bool HasAttended { get; init; }
}

public record ReservationSummaryDto
{
    public Guid Id { get; init; }
    public required string UserName { get; init; }
    public required string ClassName { get; init; }
    public required DateTime ClassStartTime { get; init; }
    public required string Status { get; init; }
    public bool HasAttended { get; init; }
}
