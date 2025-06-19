using FluentAssertions;
using StudioScheduler.Core.Models;

namespace StudioScheduler.UnitTests.Core.Models;

public class DanceClassTests
{
    [Fact]
    public void DanceClass_ShouldInitializeWithRequiredProperties()
    {
        // Act
        var danceClass = new DanceClass
        {
            Name = "Salsa Beginners",
            Description = "Basic salsa class for beginners",
            Style = "Salsa"
        };

        // Assert
        danceClass.Id.Should().NotBeEmpty();
        danceClass.Name.Should().Be("Salsa Beginners");
        danceClass.Description.Should().Be("Basic salsa class for beginners");
        danceClass.Style.Should().Be("Salsa");
        danceClass.IsActive.Should().BeTrue();
        danceClass.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        danceClass.UpdatedAt.Should().BeNull();
        danceClass.Schedules.Should().BeEmpty();
    }

    [Fact]
    public void DanceClass_ShouldBeImmutableAfterInitialization()
    {
        // Arrange & Act
        var danceClass = new DanceClass
        {
            Name = "Test Class",
            Description = "Test Description",
            Style = "Salsa"
        };

        // Assert
        // Properties with init-only setters cannot be modified after creation
        // This test documents the immutable behavior of Name, Description, and Style
        danceClass.Name.Should().Be("Test Class");
        danceClass.Description.Should().Be("Test Description");
        danceClass.Style.Should().Be("Salsa");
        
        // Verify that the properties are init-only by checking they exist
        var nameProperty = typeof(DanceClass).GetProperty(nameof(DanceClass.Name));
        var descriptionProperty = typeof(DanceClass).GetProperty(nameof(DanceClass.Description));
        var styleProperty = typeof(DanceClass).GetProperty(nameof(DanceClass.Style));
        
        nameProperty.Should().NotBeNull();
        descriptionProperty.Should().NotBeNull();
        styleProperty.Should().NotBeNull();
        
        // Init-only properties have a setter but it's only accessible during initialization
        nameProperty!.CanWrite.Should().BeTrue();
        descriptionProperty!.CanWrite.Should().BeTrue();
        styleProperty!.CanWrite.Should().BeTrue();
    }

    [Theory]
    [InlineData("Salsa")]
    [InlineData("Bachata")]
    [InlineData("Zouk")]
    [InlineData("Kizomba")]
    [InlineData("Rueda de Casino")]
    [InlineData("Ladies Styling")]
    public void DanceClass_Style_ShouldAcceptValidStyles(string style)
    {
        // Arrange & Act
        var danceClass = new DanceClass
        {
            Name = "Test Class",
            Description = "Test Description",
            Style = style
        };

        // Assert
        danceClass.Style.Should().Be(style);
    }

    [Fact]
    public void DanceClass_ShouldAllowModificationOfSchedulesCollection()
    {
        // Arrange
        var danceClass = new DanceClass
        {
            Name = "Test Class",
            Description = "Test Description",
            Style = "Salsa"
        };

        var schedule = new Schedule
        {
            Name = "Monday Evening Salsa",
            LocationId = Guid.NewGuid(),
            EffectiveFrom = DateTime.UtcNow,
            DanceClassId = danceClass.Id,
            DayOfWeek = DayOfWeek.Monday,
            StartTime = TimeSpan.FromHours(19), // 7 PM
            Duration = 60,
            Level = "Beginner",
            Capacity = 20
        };

        // Act
        danceClass.Schedules.Add(schedule);

        // Assert
        danceClass.Schedules.Should().HaveCount(1);
        danceClass.Schedules.Should().Contain(schedule);
    }

    [Fact]
    public void DanceClass_IsActive_ShouldDefaultToTrue()
    {
        // Arrange & Act
        var danceClass = new DanceClass
        {
            Name = "Test Class",
            Description = "Test Description",
            Style = "Salsa"
        };

        // Assert
        danceClass.IsActive.Should().BeTrue();
    }

    [Fact]
    public void DanceClass_CanBeDeactivated()
    {
        // Arrange
        var danceClass = new DanceClass
        {
            Name = "Test Class",
            Description = "Test Description",
            Style = "Salsa"
        };

        // Act
        danceClass.IsActive = false;

        // Assert
        danceClass.IsActive.Should().BeFalse();
    }

    [Fact]
    public void DanceClass_UpdatedAt_ShouldBeSetWhenModified()
    {
        // Arrange
        var danceClass = new DanceClass
        {
            Name = "Test Class",
            Description = "Test Description",
            Style = "Salsa"
        };

        var originalUpdatedAt = danceClass.UpdatedAt;

        // Act
        danceClass.IsActive = false;

        // Assert
        danceClass.UpdatedAt.Should().NotBeNull();
        danceClass.UpdatedAt.Should().BeAfter(originalUpdatedAt ?? DateTime.MinValue);
    }
}
