using StudioScheduler.Core.Enums;

namespace StudioScheduler.Core.Services;

public static class PassConfigurationService
{
    public static readonly Dictionary<PassType, PassConfig> Configurations = new()
    {
        [PassType.Monthly1Course] = new(1, 4),
        [PassType.Monthly2Courses] = new(2, 8),
        [PassType.Monthly3Courses] = new(3, 12),
        [PassType.Monthly4Courses] = new(4, 16),
        [PassType.Monthly5Courses] = new(5, 20),
    };

    public static bool IsMonthlyPass(PassType passType)
    {
        return passType is PassType.Monthly1Course or PassType.Monthly2Courses or 
               PassType.Monthly3Courses or PassType.Monthly4Courses or PassType.Monthly5Courses;
    }

    public static int GetClassesPerWeek(PassType passType)
    {
        return Configurations.TryGetValue(passType, out var config) ? config.ClassesPerWeek : 0;
    }

    public static int GetTotalClasses(PassType passType)
    {
        return Configurations.TryGetValue(passType, out var config) ? config.TotalClasses : 0;
    }

    public static string GetPassDisplayName(PassType passType)
    {
        return passType switch
        {
            PassType.Monthly1Course => "1 course per week (4 classes total)",
            PassType.Monthly2Courses => "2 courses per week (8 classes total)",
            PassType.Monthly3Courses => "3 courses per week (12 classes total)",
            PassType.Monthly4Courses => "4 courses per week (16 classes total)",
            PassType.Monthly5Courses => "5 courses per week (20 classes total)",
            PassType.SingleClass => "Single class",
            PassType.Flexi4Classes => "FLEXI 4 classes",
            PassType.Flexi8Classes => "FLEXI 8 classes",
            PassType.FullPass => "Full Pass (unlimited)",
            _ => passType.ToString()
        };
    }

    public static List<PassType> GetAvailableMonthlyPasses()
    {
        return new List<PassType>
        {
            PassType.Monthly1Course,
            PassType.Monthly2Courses,
            PassType.Monthly3Courses,
            PassType.Monthly4Courses,
            PassType.Monthly5Courses
        };
    }
}

public record PassConfig(int ClassesPerWeek, int TotalClasses);
