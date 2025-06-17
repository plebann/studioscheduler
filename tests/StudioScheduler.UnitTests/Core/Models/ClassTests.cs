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
            Style = "Salsa"
        };

        // Assert
        danceClass.IsActive.Should().BeTrue();
        danceClass.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        danceClass.UpdatedAt.Should().BeNull();
        danceClass.Name.Should().NotBeEmpty();
        danceClass.Description.Should().NotBeEmpty();
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
            Style = "Salsa"
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
            Style = "Salsa"
        };

        // Act
        var result = validator.Validate(danceClass);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => 
            error.ErrorMessage.Contains("Description is required"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void DanceClass_Style_CannotBeEmptyOrWhitespace(string emptyStyle)
    {
        // Arrange
        var validator = new DanceClassValidator();
        var danceClass = new DanceClass
        {
            Name = "Test Class",
            Description = "Test description",
            Style = emptyStyle
        };

        // Act
        var result = validator.Validate(danceClass);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => 
            error.ErrorMessage.Contains("Style is required"));
    }

    [Fact]
    public void DanceClass_ScheduleCollection_ShouldBeInitialized()
    {
        // Arrange & Act
        var danceClass = new DanceClass
        {
            Name = "Test Class",
            Description = "Test description",
            Style = "Salsa"
        };

        // Assert
        danceClass.Schedules.Should().NotBeNull();
        danceClass.Schedules.Should().BeEmpty();
    }
}
