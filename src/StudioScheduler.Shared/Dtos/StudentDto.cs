using StudioScheduler.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace StudioScheduler.Shared.Dtos;

public record StudentDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }  // FirstName + LastName combined
    public required string Email { get; init; }
    public string? Phone { get; init; }
    public DateTime JoinDate { get; init; }  // CreatedAt
    public bool IsActive { get; init; }
    public UserRole Role { get; init; }
}

public record CreateStudentDto
{
    [Required, EmailAddress]
    public required string Email { get; init; }
    
    [Required, MinLength(6)]
    public required string Password { get; init; }
    
    [Required, MaxLength(50)]
    public required string FirstName { get; init; }
    
    [Required, MaxLength(50)]
    public required string LastName { get; init; }
    
    [Phone]
    public string? Phone { get; init; }
    
    [MaxLength(20)]
    public string? Gender { get; init; }
    
    public DateTime? DateOfBirth { get; init; }
}

public record UpdateStudentDto
{
    [Required, MaxLength(50)]
    public required string FirstName { get; init; }
    
    [Required, MaxLength(50)]
    public required string LastName { get; init; }
    
    [Phone]
    public string? Phone { get; init; }
    
    [MaxLength(20)]
    public string? Gender { get; init; }
    
    public DateTime? DateOfBirth { get; init; }
    public bool IsActive { get; init; } = true;
}

public record StudentSummaryDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public bool IsActive { get; init; }
}
