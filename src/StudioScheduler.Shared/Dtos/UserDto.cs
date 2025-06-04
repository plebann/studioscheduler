using StudioScheduler.Core.Enums;

namespace StudioScheduler.Shared.Dtos;

public record UserDto
{
    public int Id { get; init; }
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public UserRole Role { get; init; }
    public string? PhoneNumber { get; init; }
}

public record CreateUserDto
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? PhoneNumber { get; init; }
}

public record UpdateUserDto
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? PhoneNumber { get; init; }
}

public record UserLoginDto
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

public record UserTokenDto
{
    public required string Token { get; init; }
    public required string RefreshToken { get; init; }
    public required DateTimeOffset ExpiresAt { get; init; }
}
