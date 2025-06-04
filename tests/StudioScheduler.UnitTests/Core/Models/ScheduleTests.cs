using Xunit;
using StudioScheduler.Core.Models;
using StudioScheduler.Core.Validators;

namespace StudioScheduler.UnitTests.Core.Models;

public class ScheduleTests
{
    [Fact]
    public void Schedule_Creation_Sets_Default_Values()
    {
        // Arrange & Act
        var schedule = new Schedule
        {
            ClassId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow.AddDays(1),
            Duration = TimeSpan.FromMinutes(60)
        };

        // Assert
        Assert.False(schedule.IsRecurring);
        Assert.Null(schedule.RecurrencePattern);
        Assert.Null(schedule.RecurrenceEndDate);
        Assert.False(schedule.IsCancelled);
        Assert.NotEqual(default, schedule.CreatedAt);
        Assert.Null(schedule.UpdatedAt);
    }

    [Fact]
    public void Schedule_With_Recurrence_Sets_Correct_Values()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(1);
        var endDate = startDate.AddMonths(1);

        // Act
        var schedule = new Schedule
        {
            ClassId = Guid.NewGuid(),
            StartTime = startDate,
            Duration = TimeSpan.FromMinutes(60),
            RecurrencePattern = "FREQ=WEEKLY;BYDAY=MO,WE,FR",
            RecurrenceEndDate = endDate,
            IsRecurring = true
        };

        // Assert
        Assert.True(schedule.IsRecurring);
        Assert.NotNull(schedule.RecurrencePattern);
        Assert.Equal(endDate, schedule.RecurrenceEndDate);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-30)]
    public void Schedule_Duration_Must_Be_Positive(int invalidDurationMinutes)
    {
        // Arrange
        var validator = new ScheduleValidator();
        var schedule = new Schedule
        {
            ClassId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow.AddDays(1),
            Duration = TimeSpan.FromMinutes(invalidDurationMinutes)
        };

        // Act
        var result = validator.Validate(schedule);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.ErrorMessage.Contains("Duration must be positive"));
    }

    [Fact]
    public void Schedule_StartTime_Must_Be_In_Future()
    {
        // Arrange
        var validator = new ScheduleValidator();
        var schedule = new Schedule
        {
            ClassId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow.AddMinutes(-1),
            Duration = TimeSpan.FromMinutes(60)
        };

        // Act
        var result = validator.Validate(schedule);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.ErrorMessage.Contains("Start time must be in the future"));
    }

    [Fact]
    public void Schedule_RecurringEndDate_Must_Be_After_StartTime()
    {
        // Arrange
        var validator = new ScheduleValidator();
        var startTime = DateTime.UtcNow.AddDays(1);
        var schedule = new Schedule
        {
            ClassId = Guid.NewGuid(),
            StartTime = startTime,
            Duration = TimeSpan.FromMinutes(60),
            RecurrencePattern = "FREQ=WEEKLY;BYDAY=MO",
            RecurrenceEndDate = startTime.AddDays(-1),
            IsRecurring = true
        };

        // Act
        var result = validator.Validate(schedule);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.ErrorMessage.Contains("Recurrence end date must be after start time"));
    }

    [Fact]
    public void Schedule_Must_Have_RecurrencePattern_When_IsRecurring()
    {
        // Arrange
        var validator = new ScheduleValidator();
        var schedule = new Schedule
        {
            ClassId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow.AddDays(1),
            Duration = TimeSpan.FromMinutes(60),
            IsRecurring = true,
            RecurrenceEndDate = DateTime.UtcNow.AddMonths(1)
        };

        // Act
        var result = validator.Validate(schedule);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.ErrorMessage.Contains("Recurrence pattern is required for recurring schedules"));
    }

    [Fact]
    public void Schedule_Reservations_Collection_Is_Initialized()
    {
        // Arrange & Act
        var schedule = new Schedule
        {
            ClassId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow.AddDays(1),
            Duration = TimeSpan.FromMinutes(60)
        };

        // Assert
        Assert.NotNull(schedule.Reservations);
        Assert.Empty(schedule.Reservations);
    }
}
