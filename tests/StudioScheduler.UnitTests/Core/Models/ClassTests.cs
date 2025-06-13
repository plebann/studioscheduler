using FluentAssertions;
using StudioScheduler.Core.Models;
using StudioScheduler.Core.Validators;

namespace StudioScheduler.UnitTests.Core.Models;

public class ClassTests
{
    [Fact]
    public void DanceClass_ShouldSetDefaultValues()
    {
        // Arrange & Act
        var danceClass = new DanceClass
        {
            Name = "Salsa Basics",
            Description = "Introduction to salsa",
            Level = "P1",
            Style = "Salsa",
            Capacity = 15,
            InstructorId = Guid.NewGuid(),
            RoomId = Guid.NewGuid()
        };

        // Assert
        danceClass.IsActive.Should().BeTrue();
        danceClass.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        danceClass.UpdatedAt.Should().BeNull();
        danceClass.Name.Should().NotBeEmpty();
        danceClass.Description.Should().NotBeEmpty();
        danceClass.Level.Should().NotBeEmpty();
        danceClass.Style.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void DanceClass_Name_CannotBeEmptyOrWhitespace(string emptyName)
    {
        // Arrange
        var validator = new DanceClassValidator();
        var danceClass = new DanceClass
        {
            Name = emptyName,
            Description = "Test description",
            Level = "P1",
            Style = "Salsa",
            Capacity = 15,
            InstructorId = Guid.NewGuid(),
            RoomId = Guid.NewGuid()
        };

        // Act
        var result = validator.Validate(danceClass);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => 
            error.ErrorMessage.Contains("Name is required"));
    }


    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void DanceClass_Description_CannotBeEmptyOrWhitespace(string emptyDescription)
    {
        // Arrange
        var validator = new DanceClassValidator();
        var danceClass = new DanceClass
        {
            Name = "Test Class",
            Description = emptyDescription,
            Level = "P1",
            Style = "Salsa",
            Capacity = 15,
            InstructorId = Guid.NewGuid(),
            RoomId = Guid.NewGuid()
        };

        // Act
        var result = validator.Validate(danceClass);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => 
            error.ErrorMessage.Contains("Description is required"));
    }


    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void DanceClass_Capacity_MustBePositive(int invalidCapacity)
    {
        // Arrange
        var validator = new DanceClassValidator();
        var danceClass = new DanceClass
        {
            Name = "Test Class",
            Description = "Test description",
            Level = "P1",
            Style = "Salsa",
            Capacity = invalidCapacity,
            InstructorId = Guid.NewGuid(),
            RoomId = Guid.NewGuid()
        };

        // Act
        var result = validator.Validate(danceClass);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => 
            error.ErrorMessage.Contains("Capacity must be positive"));
    }

    [Fact]
    public void DanceClass_MustHaveInstructor()
    {
        // Arrange
        var validator = new DanceClassValidator();
        var danceClass = new DanceClass
        {
            Name = "Test Class",
            Description = "Test description",
            Level = "P1",
            Style = "Salsa",
            Capacity = 15,
            InstructorId = default,
            RoomId = Guid.NewGuid()
        };

        // Act
        var result = validator.Validate(danceClass);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => 
            error.ErrorMessage.Contains("Instructor is required"));
    }

    [Fact]
    public void DanceClass_MustHaveRoom()
    {
        // Arrange
        var validator = new DanceClassValidator();
        var danceClass = new DanceClass
        {
            Name = "Test Class",
            Description = "Test description",
            Level = "P1",
            Style = "Salsa",
            Capacity = 15,
            InstructorId = Guid.NewGuid(),
            RoomId = default
        };

        // Act
        var result = validator.Validate(danceClass);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => 
            error.ErrorMessage.Contains("Room is required"));
    }

    [Fact]
    public void DanceClass_ScheduleCollection_ShouldBeInitialized()
    {
        // Arrange & Act
        var danceClass = new DanceClass
        {
            Name = "Test Class",
            Description = "Test description",
            Level = "P1",
            Style = "Salsa",
            Capacity = 15,
            InstructorId = Guid.NewGuid(),
            RoomId = Guid.NewGuid()
        };

        // Assert
        danceClass.Schedules.Should().NotBeNull();
        danceClass.Schedules.Should().BeEmpty();
    }
}
