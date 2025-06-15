namespace StudioScheduler.Shared.Dtos;

public record RoomDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required Guid LocationId { get; init; }
    public string? LocationName { get; init; }
    public int Capacity { get; init; }
    public string? Description { get; init; }
    public IEnumerable<string>? Equipment { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record CreateRoomDto
{
    public required string Name { get; init; }
    public required Guid LocationId { get; init; }
    public required int Capacity { get; init; }
    public string? Description { get; init; }
    public IEnumerable<string>? Equipment { get; init; }
}

public record UpdateRoomDto
{
    public required string Name { get; init; }
    public required int Capacity { get; init; }
    public string? Description { get; init; }
    public IEnumerable<string>? Equipment { get; init; }
}

public record RoomSummaryDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public string? LocationName { get; init; }
    public int Capacity { get; init; }
}
