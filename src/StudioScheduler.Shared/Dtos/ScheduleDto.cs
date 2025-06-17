namespace StudioScheduler.Shared.Dtos;

public record ScheduleDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required Guid LocationId { get; init; }
    public string? LocationName { get; init; }
    public required Guid DanceClassId { get; init; }
    public string? DanceClassName { get; init; }
    public required DateTime StartTime { get; init; }
    public required TimeSpan Duration { get; init; }
    public bool IsRecurring { get; init; }
    public string? RecurrencePattern { get; init; }
    public DateTime? RecurrenceEndDate { get; init; }
    public bool IsCancelled { get; init; }
    public required DateTime EffectiveFrom { get; init; }
    public DateTime? EffectiveTo { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    
    // ✅ NEW: Properties moved from DanceClass
    public required string Level { get; init; }
    public Guid? InstructorId { get; init; }
    public string? InstructorName { get; init; }
    public Guid? RoomId { get; init; }
    public string? RoomName { get; init; }
    public required int Capacity { get; init; }
}

public record CreateScheduleDto
{
    public required string Name { get; init; }
    public required Guid LocationId { get; init; }
    public required Guid DanceClassId { get; init; }
    public required DateTime StartTime { get; init; }
    public required TimeSpan Duration { get; init; }
    public bool IsRecurring { get; init; }
    public string? RecurrencePattern { get; init; }
    public DateTime? RecurrenceEndDate { get; init; }
    public required DateTime EffectiveFrom { get; init; }
    public DateTime? EffectiveTo { get; init; }
    
    // ✅ NEW: Properties moved from DanceClass
    public required string Level { get; init; }
    public Guid? InstructorId { get; init; }
    public Guid? RoomId { get; init; }
    public required int Capacity { get; init; }
}

public record UpdateScheduleDto
{
    public required string Name { get; init; }
    public required DateTime StartTime { get; init; }
    public required TimeSpan Duration { get; init; }
    public bool IsRecurring { get; init; }
    public string? RecurrencePattern { get; init; }
    public DateTime? RecurrenceEndDate { get; init; }
    public required DateTime EffectiveFrom { get; init; }
    public DateTime? EffectiveTo { get; init; }
    public bool IsActive { get; init; }
    public bool IsCancelled { get; init; }
    
    // ✅ NEW: Properties moved from DanceClass
    public required string Level { get; init; }
    public Guid? InstructorId { get; init; }
    public Guid? RoomId { get; init; }
    public required int Capacity { get; init; }
}

public record ScheduleSummaryDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public string? LocationName { get; init; }
    public string? DanceClassName { get; init; }
    public required DateTime StartTime { get; init; }
    public required TimeSpan Duration { get; init; }
    public bool IsActive { get; init; }
    public bool IsCancelled { get; init; }
    
    // ✅ NEW: Properties moved from DanceClass
    public required string Level { get; init; }
    public string? InstructorName { get; init; }
    public string? RoomName { get; init; }
    public required int Capacity { get; init; }
}

public record WeeklyScheduleDto
{
    public required Dictionary<string, List<ScheduleSlotDto>> Schedule { get; init; }
}

public record ScheduleSlotDto
{
    public Guid Id { get; init; }
    public required string TimeSlot { get; init; }
    public required string DanceName { get; init; }
    public required string Level { get; init; }
    public required string Style { get; init; }
    public required string BackgroundColor { get; init; }
    public string? EffectiveFrom { get; init; }
    public bool IsCancelled { get; init; }
    public bool IsActive { get; init; }
}
