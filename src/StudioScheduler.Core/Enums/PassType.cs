namespace StudioScheduler.Core.Enums;

/// <summary>
/// Represents the type of pass available for purchase.
/// Based on SalsaMe Dance Studio business model with 28-day validity periods.
/// </summary>
public enum PassType
{
    /// <summary>
    /// Single class pass, valid for one class only - 40 PLN
    /// Level restrictions apply (P2+ only, no beginners)
    /// </summary>
    SingleClass,
    
    /// <summary>
    /// Monthly pass: 1 course per week (4 classes total) - 130 PLN
    /// 28-day validity period from start date
    /// </summary>
    Monthly1Course,
    
    /// <summary>
    /// Monthly pass: 2 courses per week (8 classes total) - 200 PLN
    /// 28-day validity period from start date
    /// </summary>
    Monthly2Courses,
    
    /// <summary>
    /// Monthly pass: 3 courses per week (12 classes total) - 240 PLN
    /// 28-day validity period from start date
    /// </summary>
    Monthly3Courses,
    
    /// <summary>
    /// Monthly pass: 4 courses per week (16 classes total) - 280 PLN
    /// 28-day validity period from start date
    /// </summary>
    Monthly4Courses,
    
    /// <summary>
    /// Monthly pass: 5 courses per week (20 classes total) - 320 PLN
    /// 28-day validity period from start date
    /// </summary>
    Monthly5Courses,
    
    /// <summary>
    /// FLEXI pass: 4 different classes in 28 days - 140 PLN
    /// Allows 1 class of any type per week (Monday-Sunday), for 4 weeks
    /// </summary>
    Flexi4Classes,
    
    /// <summary>
    /// FLEXI pass: 8 different classes in 28 days - 220 PLN
    /// Allows 2 classes of any type per week (Monday-Sunday), for 4 weeks
    /// </summary>
    Flexi8Classes,
    
    /// <summary>
    /// FULLPASS: Unlimited classes in 28 days - 350 PLN
    /// Access to all classes within validity period
    /// </summary>
    FullPass
}
