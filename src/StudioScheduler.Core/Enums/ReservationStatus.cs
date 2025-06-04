namespace StudioScheduler.Core.Enums;

/// <summary>
/// Represents the current status of a class reservation.
/// </summary>
public enum ReservationStatus
{
    /// <summary>
    /// Reservation is awaiting confirmation.
    /// </summary>
    Pending,
    
    /// <summary>
    /// Reservation has been confirmed.
    /// </summary>
    Confirmed,
    
    /// <summary>
    /// Reservation was cancelled.
    /// </summary>
    Cancelled,
    
    /// <summary>
    /// Class has been completed and attended.
    /// </summary>
    Completed,
    
    /// <summary>
    /// Student did not attend the class.
    /// </summary>
    NoShow
}
