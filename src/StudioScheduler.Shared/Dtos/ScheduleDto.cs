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

public record ScheduleEditDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid LocationId { get; set; }
    public string? LocationName { get; set; }
    public Guid DanceClassId { get; set; }
    public string? DanceClassName { get; set; }
    public DateTime StartTime { get; set; }
    public TimeSpan Duration { get; set; }
    public bool IsRecurring { get; set; }
    public string? RecurrencePattern { get; set; }
    public DateTime? RecurrenceEndDate { get; set; }
    public bool IsCancelled { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public string Level { get; set; } = string.Empty;
    public Guid? InstructorId { get; set; }
    public string? InstructorName { get; set; }
    public Guid? RoomId { get; set; }
    public string? RoomName { get; set; }
    public int Capacity { get; set; }

    public static ScheduleEditDto FromScheduleDto(ScheduleDto dto)
    {
        return new ScheduleEditDto
        {
            Id = dto.Id,
            Name = dto.Name,
            LocationId = dto.LocationId,
            LocationName = dto.LocationName,
            DanceClassId = dto.DanceClassId,
            DanceClassName = dto.DanceClassName,
            StartTime = dto.StartTime,
            Duration = dto.Duration,
            IsRecurring = dto.IsRecurring,
            RecurrencePattern = dto.RecurrencePattern,
            RecurrenceEndDate = dto.RecurrenceEndDate,
            IsCancelled = dto.IsCancelled,
            EffectiveFrom = dto.EffectiveFrom,
            EffectiveTo = dto.EffectiveTo,
            IsActive = dto.IsActive,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            Level = dto.Level,
            InstructorId = dto.InstructorId,
            InstructorName = dto.InstructorName,
            RoomId = dto.RoomId,
            RoomName = dto.RoomName,
            Capacity = dto.Capacity
        };
    }

    public ScheduleDto ToScheduleDto()
    {
        return new ScheduleDto
        {
            Id = Id,
            Name = Name,
            LocationId = LocationId,
            LocationName = LocationName,
            DanceClassId = DanceClassId,
            DanceClassName = DanceClassName,
            StartTime = StartTime,
            Duration = Duration,
            IsRecurring = IsRecurring,
            RecurrencePattern = RecurrencePattern,
            RecurrenceEndDate = RecurrenceEndDate,
            IsCancelled = IsCancelled,
            EffectiveFrom = EffectiveFrom,
            EffectiveTo = EffectiveTo,
            IsActive = IsActive,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt,
            Level = Level,
            InstructorId = InstructorId,
            InstructorName = InstructorName,
            RoomId = RoomId,
            RoomName = RoomName,
            Capacity = Capacity
        };
    }
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
