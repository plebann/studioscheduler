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
    [InlineData("   ")]
    public void Class_Name_Cannot_Be_Empty_Or_Whitespace(string emptyName)
    {
        // Arrange
        var validator = new ClassValidator();
        var @class = new Class
        {
            Name = emptyName,
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

    [Fact]
    public void Class_Name_Cannot_Be_Null()
    {
        // Arrange
        var validator = new ClassValidator();
        var @class = new Class
        {
            Name = "Initial Name",
            Description = "Test description",
            Capacity = 15,
            InstructorId = Guid.NewGuid()
        };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
        {
            @class.Name = default!;
        });
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Class_Description_Cannot_Be_Empty_Or_Whitespace(string emptyDescription)
    {
        // Arrange
        var validator = new ClassValidator();
        var @class = new Class
        {
            Name = "Test Class",
            Description = emptyDescription,
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

    [Fact]
    public void Class_Description_Cannot_Be_Null()
    {
        // Arrange
        var validator = new ClassValidator();
        var @class = new Class
        {
            Name = "Test Class",
            Description = "Initial Description",
            Capacity = 15,
            InstructorId = Guid.NewGuid()
        };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
        {
            @class.Description = default!;
        });
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
