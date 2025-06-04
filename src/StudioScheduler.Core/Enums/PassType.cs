namespace StudioScheduler.Core.Enums;

/// <summary>
/// Represents the type of pass available for purchase.
/// </summary>
public enum PassType
{
    /// <summary>
    /// Weekly pass with a fixed number of classes per week.
    /// </summary>
    Weekly,
    
    /// <summary>
    /// Monthly pass with a fixed number of classes per month.
    /// </summary>
    Monthly,
    
    /// <summary>
    /// Quarterly pass (3 months) with a fixed number of classes.
    /// </summary>
    Quarterly,
    
    /// <summary>
    /// Annual pass with a fixed number of classes per year.
    /// </summary>
    Annual,
    
    /// <summary>
    /// Single class pass, valid for one class only.
    /// </summary>
    SingleClass
}
