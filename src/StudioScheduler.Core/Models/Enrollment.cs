using StudioScheduler.Core.Interfaces.Repositories;

namespace StudioScheduler.Core.Models;

public class Enrollment
{
    public Guid Id { get; set; }
    public required Guid StudentId { get; set; }
    public Student? Student { get; set; }
    public required Guid ScheduleId { get; set; }
    public Schedule? Schedule { get; set; }
    public required DateTime EnrolledDate { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? PassId { get; set; }
    public Pass? Pass { get; set; }
}

/// <summary>
/// Determines if an enrollment is currently valid by checking if the student has any active pass covering the schedule.
/// </summary>
public static class EnrollmentBusinessLogic
{
    public static async Task<bool> IsEnrollmentActiveAsync(Guid studentId, IPassRepository passRepository)
    {
        var activePasses = await passRepository.GetActivePassesAsync();
        var now = DateTime.UtcNow;
        return activePasses.Any(p => p.UserId == studentId && p.IsActive && p.StartDate <= now && p.EndDate >= now);
    }
}
