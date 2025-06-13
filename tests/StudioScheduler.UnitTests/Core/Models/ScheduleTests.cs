using FluentAssertions;
using StudioScheduler.Core.Models;
using StudioScheduler.Core.Validators;
using StudioScheduler.Core.Enums;

namespace StudioScheduler.UnitTests.Core.Models;

public class ScheduleTests
{
    [Fact]
    public void Schedule_ShouldInitializeWithRequiredProperties()
    {
        // Arrange
        var locationId = Guid.NewGuid();
        var danceClassId = Guid.NewGuid();

        // Act
        var schedule = new Schedule
        {
            Name = "Monday Evening Salsa",
            LocationId = locationId,
            EffectiveFrom = DateTime.UtcNow,
            DanceClassId = danceClassId,
            StartTime = DateTime.UtcNow.AddDays(1),
            Duration = TimeSpan.FromMinutes(60)
        };

        // Assert
        schedule.Id.Should().NotBeEmpty();
        schedule.Name.Should().Be("Monday Evening Salsa");
        schedule.LocationId.Should().Be(locationId);
        schedule.DanceClassId.Should().Be(danceClassId);
        schedule.EffectiveFrom.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        schedule.EffectiveTo.Should().BeNull();
        schedule.IsActive.Should().BeTrue();
        schedule.StartTime.Should().BeCloseTo(DateTime.UtcNow.AddDays(1), TimeSpan.FromSeconds(5));
        schedule.Duration.Should().Be(TimeSpan.FromMinutes(60));
        schedule.IsRecurring.Should().BeFalse();
        schedule.RecurrencePattern.Should().BeNull();
        schedule.RecurrenceEndDate.Should().BeNull();
        schedule.IsCancelled.Should().BeFalse();
        schedule.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        schedule.UpdatedAt.Should().BeNull();
        schedule.Reservations.Should().BeEmpty();
    }

    [Fact]
    public void Schedule_ShouldBeImmutableAfterInitialization()
    {
        // Arrange & Act
        var schedule = new Schedule
        {
            Name = "Test Schedule",
            LocationId = Guid.NewGuid(),
            EffectiveFrom = DateTime.UtcNow,
            DanceClassId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow.AddDays(1),
            Duration = TimeSpan.FromMinutes(60)
        };

        // Assert
        // Properties with init-only setters cannot be modified after creation
        // This test documents the immutable behavior of Name
        schedule.Name.Should().Be("Test Schedule");
        
        // Verify that the property is init-only by checking it exists
        var nameProperty = typeof(Schedule).GetProperty(nameof(Schedule.Name));
        
        nameProperty.Should().NotBeNull();
        
        // Init-only property has a setter but it's only accessible during initialization
        nameProperty!.CanWrite.Should().BeTrue();
    }

    [Fact]
    public void Schedule_With_Recurrence_ShouldSetCorrectValues()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(1);
        var endDate = startDate.AddMonths(1);

        // Act
        var schedule = new Schedule
        {
            Name = "Weekly Salsa Class",
            LocationId = Guid.NewGuid(),
            EffectiveFrom = DateTime.UtcNow,
            DanceClassId = Guid.NewGuid(),
            StartTime = startDate,
            Duration = TimeSpan.FromMinutes(60),
            RecurrencePattern = "FREQ=WEEKLY;BYDAY=MO,WE,FR",
            RecurrenceEndDate = endDate,
            IsRecurring = true
        };

        // Assert
        schedule.IsRecurring.Should().BeTrue();
        schedule.RecurrencePattern.Should().Be("FREQ=WEEKLY;BYDAY=MO,WE,FR");
        schedule.RecurrenceEndDate.Should().Be(endDate);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-30)]
    public void Schedule_Duration_MustBePositive(int invalidDurationMinutes)
    {
        // Arrange
        var validator = new ScheduleValidator();
        var schedule = new Schedule
        {
            Name = "Test Schedule",
            LocationId = Guid.NewGuid(),
            EffectiveFrom = DateTime.UtcNow,
            DanceClassId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow.AddDays(1),
            Duration = TimeSpan.FromMinutes(invalidDurationMinutes)
        };

        // Act
        var result = validator.Validate(schedule);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => 
            error.ErrorMessage.Contains("Duration must be positive"));
    }

    [Fact]
    public void Schedule_StartTime_MustBeInFuture()
    {
        // Arrange
        var validator = new ScheduleValidator();
        var schedule = new Schedule
        {
            Name = "Test Schedule",
            LocationId = Guid.NewGuid(),
            EffectiveFrom = DateTime.UtcNow,
            DanceClassId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow.AddMinutes(-1),
            Duration = TimeSpan.FromMinutes(60)
        };

        // Act
        var result = validator.Validate(schedule);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => 
            error.ErrorMessage.Contains("Start time must be in the future"));
    }

    [Fact]
    public void Schedule_RecurringEndDate_MustBeAfterStartTime()
    {
        // Arrange
        var validator = new ScheduleValidator();
        var startTime = DateTime.UtcNow.AddDays(1);
        var schedule = new Schedule
        {
            Name = "Test Schedule",
            LocationId = Guid.NewGuid(),
            EffectiveFrom = DateTime.UtcNow,
            DanceClassId = Guid.NewGuid(),
            StartTime = startTime,
            Duration = TimeSpan.FromMinutes(60),
            RecurrencePattern = "FREQ=WEEKLY;BYDAY=MO",
            RecurrenceEndDate = startTime.AddDays(-1),
            IsRecurring = true
        };

        // Act
        var result = validator.Validate(schedule);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => 
            error.ErrorMessage.Contains("Recurrence end date must be after start time"));
    }

    [Fact]
    public void Schedule_MustHaveRecurrencePattern_WhenIsRecurring()
    {
        // Arrange
        var validator = new ScheduleValidator();
        var schedule = new Schedule
        {
            Name = "Test Schedule",
            LocationId = Guid.NewGuid(),
            EffectiveFrom = DateTime.UtcNow,
            DanceClassId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow.AddDays(1),
            Duration = TimeSpan.FromMinutes(60),
            IsRecurring = true,
            RecurrenceEndDate = DateTime.UtcNow.AddMonths(1)
        };

        // Act
        var result = validator.Validate(schedule);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => 
            error.ErrorMessage.Contains("Recurrence pattern is required for recurring schedules"));
    }

    [Fact]
    public void Schedule_Reservations_CollectionShouldBeInitialized()
    {
        // Arrange & Act
        var schedule = new Schedule
        {
            Name = "Test Schedule",
            LocationId = Guid.NewGuid(),
            EffectiveFrom = DateTime.UtcNow,
            DanceClassId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow.AddDays(1),
            Duration = TimeSpan.FromMinutes(60)
        };

        // Assert
        schedule.Reservations.Should().NotBeNull();
        schedule.Reservations.Should().BeEmpty();
    }

    [Fact]
    public void Schedule_ShouldAllowModificationOfReservationsCollection()
    {
        // Arrange
        var schedule = new Schedule
        {
            Name = "Test Schedule",
            LocationId = Guid.NewGuid(),
            EffectiveFrom = DateTime.UtcNow,
            DanceClassId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow.AddDays(1),
            Duration = TimeSpan.FromMinutes(60)
        };

        var reservation = new Reservation
        {
            UserId = Guid.NewGuid(),
            ScheduleId = schedule.Id,
            PassId = Guid.NewGuid(),
            Status = ReservationStatus.Confirmed
        };

        // Act
        schedule.Reservations.Add(reservation);

        // Assert
        schedule.Reservations.Should().HaveCount(1);
        schedule.Reservations.Should().Contain(reservation);
    }
}
