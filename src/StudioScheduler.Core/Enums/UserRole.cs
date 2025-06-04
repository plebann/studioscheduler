namespace StudioScheduler.Core.Enums;

/// <summary>
/// Represents the role of a user in the system.
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Administrator with full system access.
    /// </summary>
    Admin,
    
    /// <summary>
    /// Front desk staff member with limited administrative access.
    /// </summary>
    DeskPerson,
    
    /// <summary>
    /// Student/client who can book classes.
    /// </summary>
    Student
}
