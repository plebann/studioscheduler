using Xunit;
using StudioScheduler.Core.Models;
using StudioScheduler.Core.Validators;

namespace StudioScheduler.UnitTests.Core.Models;

public class ClassTests
{
    [Fact]
    public void Class_Creation_Sets_Default_Values()
    {
        // Arrange & Act
        var @class = new Class
        {
            Name = "Yoga Basics",
            Description = "Introduction to yoga",
            Capacity = 15,
            InstructorId = Guid.NewGuid()
        };

        // Assert
        Assert.True(@class.IsActive);
        Assert.NotEqual(default, @class.CreatedAt);
        Assert.Null(@class.UpdatedAt);
        Assert.NotEmpty(@class.Name);
        Assert.NotEmpty(@class.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Class_Name_Cannot_Be_Empty(string? invalidName)
    {
        // Arrange
        var validator = new ClassValidator();
        var @class = new Class
        {
            Name = invalidName,
            Description = "Test description",
            Capacity = 15,
            InstructorId = Guid.NewGuid()
        };

        // Act
        var result = validator.Validate(@class);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.ErrorMessage.Contains("Name is required"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Class_Description_Cannot_Be_Empty(string? invalidDescription)
    {
        // Arrange
        var validator = new ClassValidator();
        var @class = new Class
        {
            Name = "Test Class",
            Description = invalidDescription,
            Capacity = 15,
            InstructorId = Guid.NewGuid()
        };

        // Act
        var result = validator.Validate(@class);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.ErrorMessage.Contains("Description is required"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Class_Capacity_Must_Be_Positive(int invalidCapacity)
    {
        // Arrange
        var validator = new ClassValidator();
        var @class = new Class
        {
            Name = "Test Class",
            Description = "Test description",
            Capacity = invalidCapacity,
            InstructorId = Guid.NewGuid()
        };

        // Act
        var result = validator.Validate(@class);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.ErrorMessage.Contains("Capacity must be positive"));
    }

    [Fact]
    public void Class_Must_Have_Instructor()
    {
        // Arrange
        var validator = new ClassValidator();
        var @class = new Class
        {
            Name = "Test Class",
            Description = "Test description",
            Capacity = 15,
            InstructorId = default
        };

        // Act
        var result = validator.Validate(@class);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => 
            error.ErrorMessage.Contains("Instructor is required"));
    }

    [Fact]
    public void Class_Schedule_Collection_Is_Initialized()
    {
        // Arrange & Act
        var @class = new Class
        {
            Name = "Test Class",
            Description = "Test description",
            Capacity = 15,
            InstructorId = Guid.NewGuid()
        };

        // Assert
        Assert.NotNull(@class.Schedules);
        Assert.Empty(@class.Schedules);
    }
}
