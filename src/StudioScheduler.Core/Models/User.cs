using StudioScheduler.Core.Enums;

namespace StudioScheduler.Core.Models;

public class User
{
    public Guid Id { get; set; }
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string PasswordHash { get; init; }
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public UserRole Role { get; set; } = UserRole.Student;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
