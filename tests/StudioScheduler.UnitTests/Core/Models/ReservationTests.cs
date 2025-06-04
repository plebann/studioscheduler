using Xunit;
using StudioScheduler.Core.Models;
using StudioScheduler.Core.Enums;
using StudioScheduler.Core.Validators;

namespace StudioScheduler.UnitTests.Core.Models;

public class ReservationTests
{
    [Fact]
    public void Reservation_Creation_Sets_Default_Values()
    {
        // Arrange & Act
        var reservation = new Reservation
        {
            UserId = Guid.NewGuid(),
            ScheduleId = Guid.NewGuid(),
            PassId = Guid.NewGuid()
        };

        // Assert
        Assert.Equal(ReservationStatus.Pending, reservation.Status);
        Assert.False(reservation.HasAttended);
        Assert.NotEqual(default, reservation.CreatedAt);
        Assert.Null(reservation.UpdatedAt);
        Assert.Null(reservation.CancelledAt);
        Assert.Null(reservation.CancellationReason);
    }

    [Theory]
    [InlineData(ReservationStatus.Confirmed)]
    [InlineData(ReservationStatus.Cancelled)]
    [InlineData(ReservationStatus.Completed)]
    public void Reservation_Status_Can_Be_Updated(ReservationStatus newStatus)
    {
        // Arrange
        var validator = new ReservationValidator();
        var reservation = new Reservation
        {
            UserId = Guid.NewGuid(),
            ScheduleId = Guid.NewGuid(),
            PassId = Guid.NewGuid()
        };

        // Act
        if (newStatus == ReservationStatus.Cancelled)
        {
            reservation.CancellationReason = "Test cancellation";
        }
        reservation.Status = newStatus;

        // Assert
        Assert.Equal(newStatus, reservation.Status);
        if (newStatus == ReservationStatus.Cancelled)
        {
            var result = validator.Validate(reservation);
            Assert.True(result.IsValid);
        }
    }

    [Fact]
    public void Reservation_Cancellation_Sets_Required_Fields()
    {
        // Arrange
        var validator = new ReservationValidator();
        var reservation = new Reservation
        {
            UserId = Guid.NewGuid(),
            ScheduleId = Guid.NewGuid(),
            PassId = Guid.NewGuid()
        };

        // Act
        var reason = "Unable to attend";
        reservation.CancellationReason = reason;
        reservation.Status = ReservationStatus.Cancelled;
        reservation.CancelledAt = DateTime.UtcNow;

        // Assert
        var result = validator.Validate(reservation);
        Assert.True(result.IsValid);
        Assert.Equal(ReservationStatus.Cancelled, reservation.Status);
        Assert.Equal(reason, reservation.CancellationReason);
        Assert.NotNull(reservation.CancelledAt);
    }

    [Fact]
    public void Reservation_Cannot_Be_Cancelled_Without_Reason()
    {
        // Arrange
        var validator = new ReservationValidator();
        var reservation = new Reservation
        {
            UserId = Guid.NewGuid(),
            ScheduleId = Guid.NewGuid(),
            PassId = Guid.NewGuid()
        };

        // Act
        reservation.Status = ReservationStatus.Cancelled;
        reservation.CancelledAt = DateTime.UtcNow;

        // Assert
        var result = validator.Validate(reservation);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.ErrorMessage.Contains("Cancellation reason is required"));
    }

    [Fact]
    public void Reservation_Cannot_Set_Attendance_When_Not_Confirmed()
    {
        // Arrange
        var validator = new ReservationValidator();
        var reservation = new Reservation
        {
            UserId = Guid.NewGuid(),
            ScheduleId = Guid.NewGuid(),
            PassId = Guid.NewGuid(),
            HasAttended = true
        };

        // Act
        var result = validator.Validate(reservation);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.ErrorMessage.Contains("Cannot mark attendance for unconfirmed reservation"));
    }

    [Fact]
    public void Reservation_Cannot_Be_Created_With_Default_Ids()
    {
        // Arrange
        var validator = new ReservationValidator();

        // Act & Assert - Check UserId
        var reservationWithDefaultUserId = new Reservation
        {
            UserId = default,
            ScheduleId = Guid.NewGuid(),
            PassId = Guid.NewGuid()
        };
        var result = validator.Validate(reservationWithDefaultUserId);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.ErrorMessage.Contains("User ID is required"));

        // Check ScheduleId
        var reservationWithDefaultScheduleId = new Reservation
        {
            UserId = Guid.NewGuid(),
            ScheduleId = default,
            PassId = Guid.NewGuid()
        };
        result = validator.Validate(reservationWithDefaultScheduleId);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.ErrorMessage.Contains("Schedule ID is required"));

        // Check PassId
        var reservationWithDefaultPassId = new Reservation
        {
            UserId = Guid.NewGuid(),
            ScheduleId = Guid.NewGuid(),
            PassId = default
        };
        result = validator.Validate(reservationWithDefaultPassId);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.ErrorMessage.Contains("Pass ID is required"));
    }
}
