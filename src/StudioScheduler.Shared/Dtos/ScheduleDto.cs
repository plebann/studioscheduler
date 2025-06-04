namespace StudioScheduler.Shared.Dtos;

public record ScheduleDto
{
    public Guid Id { get; init; }
    public Guid ClassId { get; init; }
    public required string ClassName { get; init; }
    public required string InstructorName { get; init; }
    public required DateTime StartTime { get; init; }
    public required TimeSpan Duration { get; init; }
    public bool IsRecurring { get; init; }
    public string? RecurrencePattern { get; init; }
    public DateTime? RecurrenceEndDate { get; init; }
    public bool IsCancelled { get; init; }
    public int AvailableSpots { get; init; }
    public int TotalCapacity { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record CreateScheduleDto
{
    public required Guid ClassId { get; init; }
    public required DateTime StartTime { get; init; }
    public required TimeSpan Duration { get; init; }
    public bool IsRecurring { get; init; }
    public string? RecurrencePattern { get; init; }
    public DateTime? RecurrenceEndDate { get; init; }
}

public record UpdateScheduleDto
{
    public required DateTime StartTime { get; init; }
    public required TimeSpan Duration { get; init; }
    public bool IsRecurring { get; init; }
    public string? RecurrencePattern { get; init; }
    public DateTime? RecurrenceEndDate { get; init; }
    public bool IsCancelled { get; init; }
}

public record ScheduleSummaryDto
{
    public Guid Id { get; init; }
    public required string ClassName { get; init; }
    public required string InstructorName { get; init; }
    public required DateTime StartTime { get; init; }
    public required TimeSpan Duration { get; init; }
    public bool IsCancelled { get; init; }
    public int AvailableSpots { get; init; }
}
