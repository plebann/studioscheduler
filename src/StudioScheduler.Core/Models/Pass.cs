using StudioScheduler.Core.Enums;

namespace StudioScheduler.Core.Models;

public class Pass
{
    public Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public User? User { get; set; }
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
    public required PassType Type { get; set; } = PassType.SingleClass;
    public required int ClassesPerWeek { get; set; }
    public required int TotalClasses { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Rich Domain Methods

    /// <summary>
    /// Calculates remaining classes based on current date, pass validity, and usage.
    /// Week definition: Monday to Sunday
    /// </summary>
    public int CalculateRemainingClasses(IEnumerable<Attendance> attendances, DateTime currentDate = default)
    {
        if (currentDate == default) currentDate = DateTime.Today;
        
        // Pass expired or not yet started
        if (currentDate > EndDate || currentDate < StartDate) return 0;
        if (!IsActive) return 0;

        return Type switch
        {
            PassType.SingleClass => CalculateSingleClassRemaining(attendances),
            PassType.FullPass => CalculateFullPassRemaining(currentDate),
            PassType.Flexi4Classes or PassType.Flexi8Classes => CalculateFlexiPassRemaining(attendances, currentDate),
            _ => CalculateRegularPassRemaining(attendances, currentDate)
        };
    }

    /// <summary>
    /// Checks if the pass is valid on a specific date
    /// </summary>
    public bool IsValidOn(DateTime date)
    {
        return IsActive && date >= StartDate && date <= EndDate;
    }

    /// <summary>
    /// Checks if pass can be used for a class on specific date considering weekly limits
    /// </summary>
    public bool CanUseForClass(DateTime classDate, IEnumerable<Attendance> existingAttendances)
    {
        if (!IsValidOn(classDate)) return false;

        var weekStart = GetWeekStart(classDate);
        var weekAttendances = existingAttendances
            .Where(a => a.ClassDate >= weekStart && 
                       a.ClassDate < weekStart.AddDays(7) && 
                       a.WasPresent)
            .ToList();

        return Type switch
        {
            PassType.SingleClass => !existingAttendances.Any(a => a.WasPresent),
            PassType.FullPass => true, // No weekly limits
            PassType.Flexi4Classes => weekAttendances.Count < 1, // Max 1 per week
            PassType.Flexi8Classes => weekAttendances.Count < 2, // Max 2 per week
            _ => weekAttendances.Count < ClassesPerWeek // Regular passes: respect ClassesPerWeek
        };
    }

    /// <summary>
    /// Gets the number of classes used from this pass
    /// </summary>
    public int GetUsedClassesCount(IEnumerable<Attendance> attendances)
    {
        return attendances.Count(a => a.WasPresent && a.PassUsed == Id);
    }

    /// <summary>
    /// Gets classes used in current week (Monday-Sunday)
    /// </summary>
    public int GetClassesUsedThisWeek(IEnumerable<Attendance> attendances, DateTime currentDate = default)
    {
        if (currentDate == default) currentDate = DateTime.Today;
        
        var weekStart = GetWeekStart(currentDate);
        return attendances
            .Count(a => a.ClassDate >= weekStart && 
                       a.ClassDate < weekStart.AddDays(7) && 
                       a.WasPresent && 
                       a.PassUsed == Id);
    }

    /// <summary>
    /// Calculates how many complete weeks remain in the pass validity period
    /// </summary>
    public int GetCompleteWeeksRemaining(DateTime currentDate = default)
    {
        if (currentDate == default) currentDate = DateTime.Today;
        
        if (currentDate > EndDate) return 0;
        
        var daysRemaining = (EndDate - currentDate).Days + 1; // Include today
        return daysRemaining / 7;
    }

    /// <summary>
    /// Gets pass status with detailed information
    /// </summary>
    public PassStatus GetPassStatus(IEnumerable<Attendance> attendances, DateTime currentDate = default)
    {
        if (currentDate == default) currentDate = DateTime.Today;
        
        if (!IsActive) return PassStatus.Inactive;
        if (currentDate < StartDate) return PassStatus.NotYetStarted;
        if (currentDate > EndDate) return PassStatus.Expired;
        
        var usedClasses = GetUsedClassesCount(attendances);
        var remainingClasses = CalculateRemainingClasses(attendances, currentDate);
        
        if (Type != PassType.FullPass && usedClasses >= TotalClasses) 
            return PassStatus.Exhausted;
        
        if (remainingClasses == 0) return PassStatus.Exhausted;
        
        return PassStatus.Active;
    }

    /// <summary>
    /// Checks if pass allows make-up classes (missed classes can be attended later)
    /// </summary>
    public bool AllowsMakeUpClasses()
    {
        // All SalsaMe passes allow make-up classes within validity period
        return Type != PassType.SingleClass;
    }

    // Private helper methods

    private int CalculateSingleClassRemaining(IEnumerable<Attendance> attendances)
    {
        var used = attendances.Count(a => a.WasPresent && a.PassUsed == Id);
        return used >= 1 ? 0 : 1;
    }

    private int CalculateFullPassRemaining(DateTime currentDate)
    {
        // FullPass has unlimited classes until expiry
        return int.MaxValue;
    }

    private int CalculateFlexiPassRemaining(IEnumerable<Attendance> attendances, DateTime currentDate)
    {
        var usedClasses = GetUsedClassesCount(attendances);
        var remainingFromTotal = TotalClasses - usedClasses;
        
        if (remainingFromTotal <= 0) return 0;
        
        // Calculate maximum possible classes based on remaining weeks and weekly limits
        var weeksRemaining = GetCompleteWeeksRemaining(currentDate);
        var classesThisWeek = GetClassesUsedThisWeek(attendances, currentDate);
        
        var weeklyLimit = Type == PassType.Flexi4Classes ? 1 : 2;
        var possibleThisWeek = Math.Max(0, weeklyLimit - classesThisWeek);
        var possibleFutureWeeks = weeksRemaining * weeklyLimit;
        
        var maxPossible = possibleThisWeek + possibleFutureWeeks;
        
        return Math.Min(remainingFromTotal, maxPossible);
    }

    private int CalculateRegularPassRemaining(IEnumerable<Attendance> attendances, DateTime currentDate)
    {
        var usedClasses = GetUsedClassesCount(attendances);
        var remainingFromTotal = TotalClasses - usedClasses;
        
        if (remainingFromTotal <= 0) return 0;
        
        // Calculate maximum possible classes based on remaining weeks and weekly limits
        var weeksRemaining = GetCompleteWeeksRemaining(currentDate);
        var classesThisWeek = GetClassesUsedThisWeek(attendances, currentDate);
        
        var possibleThisWeek = Math.Max(0, ClassesPerWeek - classesThisWeek);
        var possibleFutureWeeks = weeksRemaining * ClassesPerWeek;
        
        var maxPossible = possibleThisWeek + possibleFutureWeeks;
        
        return Math.Min(remainingFromTotal, maxPossible);
    }

    private static DateTime GetWeekStart(DateTime date)
    {
        // Week starts on Monday
        var daysFromMonday = ((int)date.DayOfWeek - 1 + 7) % 7;
        return date.Date.AddDays(-daysFromMonday);
    }
}

/// <summary>
/// Represents the current status of a pass
/// </summary>
public enum PassStatus
{
    Active,
    Inactive,
    Expired,
    Exhausted,
    NotYetStarted
}
