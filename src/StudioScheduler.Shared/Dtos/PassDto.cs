namespace StudioScheduler.Shared.Dtos;

public record PassDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public required string UserName { get; init; }
    public required DateTime StartDate { get; init; }
    public required DateTime EndDate { get; init; }
    public required string Type { get; init; }
    public int ClassesPerWeek { get; init; }
    public int TotalClasses { get; init; }
    public int RemainingClasses { get; init; } // Calculated dynamically
    public int UsedClasses { get; init; }
    public int ClassesUsedThisWeek { get; init; }
    public int CompleteWeeksRemaining { get; init; }
    public required string PassStatus { get; init; }
    public bool AllowsMakeUpClasses { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record CreatePassDto
{
    public required Guid UserId { get; init; }
    public required DateTime StartDate { get; init; }
    public required DateTime EndDate { get; init; }
    public required string Type { get; init; }
    public int ClassesPerWeek { get; init; }
    public int TotalClasses { get; init; }
}

public record UpdatePassDto
{
    public required DateTime EndDate { get; init; }
    public required bool IsActive { get; init; }
    public int? AdditionalClasses { get; init; }
}

public record PassSummaryDto
{
    public Guid Id { get; init; }
    public required string UserName { get; init; }
    public required string Type { get; init; }
    public required DateTime EndDate { get; init; }
    public int RemainingClasses { get; init; }
    public bool IsActive { get; init; }
}

public record PassTypeDto
{
    public required string Type { get; init; }
    public required string Description { get; init; }
    public required decimal Price { get; init; }
    public int? ClassesPerWeek { get; init; }
    public int? TotalClasses { get; init; }
    public required int ValidityPeriodInDays { get; init; } // Always 28 days for SalsaMe
    public bool IsFlexiPass { get; init; }
    public bool AllowsMakeUpClasses { get; init; }
    public required string WeekDefinition { get; init; } // "Monday to Sunday"
}

public record PassUsageStatsDto
{
    public Guid PassId { get; init; }
    public int TotalClassesAttended { get; init; }
    public int ClassesThisWeek { get; init; }
    public int WeeksUsed { get; init; }
    public int WeeksRemaining { get; init; }
    public DateTime? LastAttendanceDate { get; init; }
    public List<WeeklyUsageDto> WeeklyBreakdown { get; init; } = new();
}

public record WeeklyUsageDto
{
    public DateTime WeekStartDate { get; init; } // Monday
    public int ClassesAttended { get; init; }
    public int MaxAllowedForWeek { get; init; }
    public bool IsCurrentWeek { get; init; }
}
