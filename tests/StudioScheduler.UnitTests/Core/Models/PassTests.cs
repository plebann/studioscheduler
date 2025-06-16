using Xunit;
using StudioScheduler.Core.Models;
using StudioScheduler.Core.Enums;
using StudioScheduler.Core.Validators;

namespace StudioScheduler.UnitTests.Core.Models;

public class PassTests
{
    [Fact]
    public void Pass_Creation_Sets_Default_Values()
    {
        // Arrange & Act
        var pass = new Pass
        {
            UserId = Guid.NewGuid(),
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(28),
            Type = PassType.SingleClass,
            ClassesPerWeek = 1,
            TotalClasses = 1
        };

        // Assert
        Assert.True(pass.IsActive);
        Assert.Equal(PassType.SingleClass, pass.Type);
        Assert.Equal(1, pass.CalculateRemainingClasses(new List<Attendance>()));
        Assert.Equal(1, pass.ClassesPerWeek);
    }

    [Fact]
    public void Pass_Creation_With_Monthly3Courses_Pass_Sets_Correct_Values()
    {
        // Arrange
        var startDate = DateTime.Today;
        var endDate = startDate.AddDays(28); // 28-day validity period

        // Act
        var pass = new Pass
        {
            UserId = Guid.NewGuid(),
            StartDate = startDate,
            EndDate = endDate,
            Type = PassType.Monthly3Courses,
            ClassesPerWeek = 3,
            TotalClasses = 12
        };

        // Assert
        Assert.True(pass.IsActive);
        Assert.Equal(PassType.Monthly3Courses, pass.Type);
        Assert.Equal(12, pass.CalculateRemainingClasses(new List<Attendance>())); // 3 classes/week * 4 weeks
        Assert.Equal(3, pass.ClassesPerWeek);
    }

    [Theory]
    [InlineData(PassType.SingleClass, 1, 1)]
    [InlineData(PassType.Monthly3Courses, 3, 12)]
    [InlineData(PassType.Monthly5Courses, 5, 20)]
    public void Pass_Creation_Sets_Correct_RemainingClasses(PassType passType, int classesPerWeek, int expectedClasses)
    {
        // Arrange & Act
        var pass = new Pass
        {
            UserId = Guid.NewGuid(),
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(28),
            Type = passType,
            ClassesPerWeek = classesPerWeek,
            TotalClasses = expectedClasses
        };

        // Assert
        Assert.Equal(expectedClasses, pass.CalculateRemainingClasses(new List<Attendance>()));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Pass_ClassesPerWeek_Must_Be_Positive(int invalidClassesPerWeek)
    {
        // Arrange
        var validator = new PassValidator();
        var pass = new Pass
        {
            UserId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30),
            Type = PassType.Monthly3Courses,
            ClassesPerWeek = invalidClassesPerWeek,
            TotalClasses = 12
        };

        // Act
        var result = validator.Validate(pass);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.ErrorMessage.Contains("Classes per week must be positive"));
    }

    [Fact]
    public void Pass_EndDate_Must_Be_After_StartDate()
    {
        // Arrange
        var validator = new PassValidator();
        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(-1);
        var pass = new Pass
        {
            UserId = Guid.NewGuid(),
            StartDate = startDate,
            EndDate = endDate,
            Type = PassType.Monthly3Courses,
            ClassesPerWeek = 3,
            TotalClasses = 12
        };

        // Act
        var result = validator.Validate(pass);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.ErrorMessage.Contains("End date must be after start date"));
    }
}
