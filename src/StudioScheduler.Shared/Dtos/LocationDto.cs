namespace StudioScheduler.Shared.Dtos;

public record LocationDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Address { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; }
    public int Capacity { get; init; }
    public TimeSpan OpeningTime { get; init; }
    public TimeSpan ClosingTime { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record CreateLocationDto
{
    public required string Name { get; init; }
    public required string Address { get; init; }
    public string? Description { get; init; }
    public required int Capacity { get; init; }
    public required TimeSpan OpeningTime { get; init; }
    public required TimeSpan ClosingTime { get; init; }
}

public record UpdateLocationDto
{
    public required string Name { get; init; }
    public required string Address { get; init; }
    public string? Description { get; init; }
    public required int Capacity { get; init; }
    public required TimeSpan OpeningTime { get; init; }
    public required TimeSpan ClosingTime { get; init; }
    public required bool IsActive { get; init; }
}

public record LocationSummaryDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Address { get; init; }
    public int Capacity { get; init; }
    public bool IsActive { get; init; }
}
