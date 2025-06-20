using StudioScheduler.Core.Enums;

namespace StudioScheduler.Shared.Dtos;

public record BuyPassRequestDto
{
    public required Guid StudentId { get; init; }
    public required PassType PassType { get; init; }
    public required DateTime StartDate { get; init; }
    public required List<Guid> SelectedScheduleIds { get; init; }
}

public record ScheduleSelectionDto
{
    public Guid ScheduleId { get; init; }
    public required string DisplayText { get; init; } // "Monday 18:00 - Bachata P1"
    public required DayOfWeek DayOfWeek { get; init; }
    public required TimeSpan StartTime { get; init; }
    public required string DanceClass { get; init; }
    public required string Level { get; init; }
    public string? InstructorName { get; init; }
    public bool IsSelected { get; set; }
}

public record PassPurchaseResponseDto
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public Guid? PassId { get; init; }
}
