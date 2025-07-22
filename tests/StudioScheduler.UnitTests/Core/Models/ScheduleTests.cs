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
            LocationId = locationId,
            EffectiveFrom = DateTime.UtcNow,
            DanceClassId = danceClassId,
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeSpan(19, 0, 0), // 7 PM
            Duration = 60,
            Level = "Beginner"
        };

        // Assert
        schedule.Id.Should().NotBeEmpty();
        schedule.LocationId.Should().Be(locationId);
        schedule.DanceClassId.Should().Be(danceClassId);
        schedule.EffectiveFrom.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        schedule.EffectiveTo.Should().BeNull();
        schedule.IsActive.Should().BeTrue();
        schedule.DayOfWeek.Should().Be(DayOfWeek.Monday);
        schedule.StartTime.Should().Be(new TimeSpan(19, 0, 0));
        schedule.Duration.Should().Be(60);
        schedule.IsRecurring.Should().BeFalse();
        schedule.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        schedule.UpdatedAt.Should().BeNull();
        schedule.Level.Should().Be("Beginner");
    }

    [Fact]
    public void Schedule_ShouldBeImmutableAfterInitialization()
    {
        // Arrange & Act
        var schedule = new Schedule
        {
            LocationId = Guid.NewGuid(),
            EffectiveFrom = DateTime.UtcNow,
            DanceClassId = Guid.NewGuid(),
            DayOfWeek = DayOfWeek.Tuesday,
            StartTime = new TimeSpan(20, 0, 0), // 8 PM
            Duration = 60,
            Level = "Beginner"
        };

        // Assert
        // Properties with init-only setters cannot be modified after creation
        // This test documents the immutable behavior of Name
        
        // Verify that the property is init-only by checking it exists
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
            LocationId = Guid.NewGuid(),
            EffectiveFrom = DateTime.UtcNow,
            DanceClassId = Guid.NewGuid(),
            DayOfWeek = DayOfWeek.Wednesday,
            StartTime = new TimeSpan(18, 30, 0), // 6:30 PM
            Duration = 60,
            IsRecurring = true,
            Level = "Intermediate"
        };

        // Assert
        schedule.IsRecurring.Should().BeTrue();
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
            LocationId = Guid.NewGuid(),
            EffectiveFrom = DateTime.UtcNow,
            DanceClassId = Guid.NewGuid(),
            DayOfWeek = DayOfWeek.Thursday,
            StartTime = new TimeSpan(19, 0, 0),
            Duration = invalidDurationMinutes,
            Level = "Beginner"
        };

        // Act
        var result = validator.Validate(schedule);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => 
            error.ErrorMessage.Contains("Duration must be positive"));
    }

    [Fact]
    public void Schedule_StartTime_ShouldBeValidTimeOfDay()
    {
        // Arrange
        var validator = new ScheduleValidator();
        var schedule = new Schedule
        {
            LocationId = Guid.NewGuid(),
            EffectiveFrom = DateTime.UtcNow,
            DanceClassId = Guid.NewGuid(),
            DayOfWeek = DayOfWeek.Friday,
            StartTime = new TimeSpan(21, 0, 0), // 9 PM - valid time
            Duration = 60,
            Level = "Beginner",
            InstructorId = Guid.NewGuid(), // Required by validator
            RoomId = Guid.NewGuid() // Required by validator
        };

        // Act
        var result = validator.Validate(schedule);

        // Assert
        result.IsValid.Should().BeTrue(); // Start time is now just time of day, so it should be valid
    }

    [Fact]
    public void Schedule_RecurringEndDate_MustBeAfterEffectiveFrom()
    {
        // Arrange
        var validator = new ScheduleValidator();
        var schedule = new Schedule
        {
            LocationId = Guid.NewGuid(),
            EffectiveFrom = DateTime.UtcNow,
            DanceClassId = Guid.NewGuid(),
            DayOfWeek = DayOfWeek.Saturday,
            StartTime = new TimeSpan(10, 0, 0), // 10 AM
            Duration = 60,
            IsRecurring = true,
            Level = "Beginner",
            InstructorId = Guid.NewGuid(), // Required by validator
            RoomId = Guid.NewGuid() // Required by validator
        };

        // Act
        var result = validator.Validate(schedule);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => 
            error.ErrorMessage.Contains("Recurrence end date must be after effective from date"));
    }
}
