namespace StudioScheduler.Shared.Dtos;

public record ClassDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public int Capacity { get; init; }
    public Guid? InstructorId { get; init; }
    public string? InstructorName { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record CreateClassDto
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Level { get; init; }
    public required string Style { get; init; }
    public required int Capacity { get; init; }
    public Guid? InstructorId { get; init; }
    public Guid? RoomId { get; init; }
}

public record UpdateClassDto
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required int Capacity { get; init; }
    public Guid? InstructorId { get; init; }
    public required bool IsActive { get; init; }
    public required string Level { get; init; }
    public required string Style { get; init; }
    public Guid? RoomId { get; init; }
}

public record ClassSummaryDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public string? InstructorName { get; init; }
    public int Capacity { get; init; }
    public bool IsActive { get; init; }
}
