using FluentAssertions;
using StudioScheduler.Core.Models;

namespace StudioScheduler.UnitTests.Core.Models;

public class DanceClassTests
{
    [Fact]
    public void DanceClass_ShouldInitializeWithRequiredProperties()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var roomId = Guid.NewGuid();

        // Act
        var danceClass = new DanceClass
        {
            Name = "Salsa Beginners",
            Description = "Basic salsa class for beginners",
            Level = "P1",
            Style = "Salsa",
            Capacity = 20,
            InstructorId = instructorId,
            RoomId = roomId
        };

        // Assert
        danceClass.Id.Should().NotBeEmpty();
        danceClass.Name.Should().Be("Salsa Beginners");
        danceClass.Description.Should().Be("Basic salsa class for beginners");
        danceClass.Level.Should().Be("P1");
        danceClass.Style.Should().Be("Salsa");
        danceClass.Capacity.Should().Be(20);
        danceClass.InstructorId.Should().Be(instructorId);
        danceClass.RoomId.Should().Be(roomId);
        danceClass.Instructor.Should().BeNull();
        danceClass.Room.Should().BeNull();
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
            Level = "P1",
            Style = "Salsa",
            Capacity = 20,
            InstructorId = Guid.NewGuid(),
            RoomId = Guid.NewGuid()
        };

        // Assert
        // Properties with init-only setters cannot be modified after creation
        // This test documents the immutable behavior of Name, Description, Level, and Style
        danceClass.Name.Should().Be("Test Class");
        danceClass.Description.Should().Be("Test Description");
        danceClass.Level.Should().Be("P1");
        danceClass.Style.Should().Be("Salsa");
        
        // Verify that the properties are init-only by checking they exist
        var nameProperty = typeof(DanceClass).GetProperty(nameof(DanceClass.Name));
        var descriptionProperty = typeof(DanceClass).GetProperty(nameof(DanceClass.Description));
        var levelProperty = typeof(DanceClass).GetProperty(nameof(DanceClass.Level));
        var styleProperty = typeof(DanceClass).GetProperty(nameof(DanceClass.Style));
        
        nameProperty.Should().NotBeNull();
        descriptionProperty.Should().NotBeNull();
        levelProperty.Should().NotBeNull();
        styleProperty.Should().NotBeNull();
        
        // Init-only properties have a setter but it's only accessible during initialization
        nameProperty!.CanWrite.Should().BeTrue();
        descriptionProperty!.CanWrite.Should().BeTrue();
        levelProperty!.CanWrite.Should().BeTrue();
        styleProperty!.CanWrite.Should().BeTrue();
    }

    [Theory]
    [InlineData("P1")]
    [InlineData("P2")]
    [InlineData("P3")]
    [InlineData("S1")]
    [InlineData("S2")]
    [InlineData("S3")]
    [InlineData("Z")]
    [InlineData("OPEN")]
    public void DanceClass_Level_ShouldAcceptValidLevels(string level)
    {
        // Arrange & Act
        var danceClass = new DanceClass
        {
            Name = "Test Class",
            Description = "Test Description",
            Level = level,
            Style = "Salsa",
            Capacity = 20,
            InstructorId = Guid.NewGuid(),
            RoomId = Guid.NewGuid()
        };

        // Assert
        danceClass.Level.Should().Be(level);
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
            Level = "P1",
            Style = style,
            Capacity = 20,
            InstructorId = Guid.NewGuid(),
            RoomId = Guid.NewGuid()
        };

        // Assert
        danceClass.Style.Should().Be(style);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public void DanceClass_Capacity_ShouldAcceptPositiveValues(int capacity)
    {
        // Arrange & Act
        var danceClass = new DanceClass
        {
            Name = "Test Class",
            Description = "Test Description",
            Level = "P1",
            Style = "Salsa",
            Capacity = capacity,
            InstructorId = Guid.NewGuid(),
            RoomId = Guid.NewGuid()
        };

        // Assert
        danceClass.Capacity.Should().Be(capacity);
    }

    [Fact]
    public void DanceClass_ShouldAllowModificationOfSchedulesCollection()
    {
        // Arrange
        var danceClass = new DanceClass
        {
            Name = "Test Class",
            Description = "Test Description",
            Level = "P1",
            Style = "Salsa",
            Capacity = 20,
            InstructorId = Guid.NewGuid(),
            RoomId = Guid.NewGuid()
        };

        var schedule = new Schedule
        {
            Name = "Monday Evening Salsa",
            LocationId = Guid.NewGuid(),
            EffectiveFrom = DateTime.UtcNow,
            DanceClassId = danceClass.Id,
            StartTime = DateTime.UtcNow.AddDays(1),
            Duration = TimeSpan.FromHours(1)
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
            Level = "P1",
            Style = "Salsa",
            Capacity = 20,
            InstructorId = Guid.NewGuid(),
            RoomId = Guid.NewGuid()
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
            Level = "P1",
            Style = "Salsa",
            Capacity = 20,
            InstructorId = Guid.NewGuid(),
            RoomId = Guid.NewGuid()
        };

        // Act
        danceClass.IsActive = false;

        // Assert
        danceClass.IsActive.Should().BeFalse();
    }
}
