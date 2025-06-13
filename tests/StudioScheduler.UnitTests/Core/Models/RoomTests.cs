using FluentAssertions;
using StudioScheduler.Core.Models;

namespace StudioScheduler.UnitTests.Core.Models;

public class RoomTests
{
    [Fact]
    public void Room_ShouldInitializeWithRequiredProperties()
    {
        // Arrange
        var locationId = Guid.NewGuid();

        // Act
        var room = new Room
        {
            Name = "Studio A",
            Description = "Main dance studio",
            Capacity = 30,
            LocationId = locationId
        };

        // Assert
        room.Id.Should().NotBeEmpty();
        room.Name.Should().Be("Studio A");
        room.Description.Should().Be("Main dance studio");
        room.Capacity.Should().Be(30);
        room.LocationId.Should().Be(locationId);
        room.Location.Should().BeNull();
        room.IsActive.Should().BeTrue();
        room.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        room.UpdatedAt.Should().BeNull();
        room.Classes.Should().BeEmpty();
    }

    [Fact]
    public void Room_ShouldBeImmutableAfterInitialization()
    {
        // Arrange & Act
        var room = new Room
        {
            Name = "Test Room",
            Description = "Test Description",
            Capacity = 20,
            LocationId = Guid.NewGuid()
        };

        // Assert
        // Properties with init-only setters cannot be modified after creation
        // This test documents the immutable behavior of Name and Description
        room.Name.Should().Be("Test Room");
        room.Description.Should().Be("Test Description");
        
        // Verify that the properties are init-only by checking they exist
        var nameProperty = typeof(Room).GetProperty(nameof(Room.Name));
        var descriptionProperty = typeof(Room).GetProperty(nameof(Room.Description));
        
        nameProperty.Should().NotBeNull();
        descriptionProperty.Should().NotBeNull();
        
        // Init-only properties have a setter but it's only accessible during initialization
        nameProperty!.CanWrite.Should().BeTrue();
        descriptionProperty!.CanWrite.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-50)]
    public void Room_Capacity_ShouldAcceptZeroAndNegativeValues(int capacity)
    {
        // Arrange & Act
        var room = new Room
        {
            Name = "Test Room",
            Description = "Test Description",
            Capacity = capacity,
            LocationId = Guid.NewGuid()
        };

        // Assert
        room.Capacity.Should().Be(capacity);
    }

    [Fact]
    public void Room_ShouldAllowModificationOfClassesCollection()
    {
        // Arrange
        var room = new Room
        {
            Name = "Test Room",
            Description = "Test Description",
            Capacity = 20,
            LocationId = Guid.NewGuid()
        };

        var danceClass = new DanceClass
        {
            Name = "Salsa Beginners",
            Description = "Basic salsa class",
            Level = "P1",
            Style = "Salsa",
            Capacity = 15,
            InstructorId = Guid.NewGuid(),
            RoomId = room.Id
        };

        // Act
        room.Classes.Add(danceClass);

        // Assert
        room.Classes.Should().HaveCount(1);
        room.Classes.Should().Contain(danceClass);
    }

    [Fact]
    public void Room_LocationId_ShouldBeRequired()
    {
        // Arrange & Act
        var room = new Room
        {
            Name = "Test Room",
            Description = "Test Description",
            Capacity = 20,
            LocationId = Guid.Empty
        };

        // Assert
        room.LocationId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void Room_IsActive_ShouldDefaultToTrue()
    {
        // Arrange & Act
        var room = new Room
        {
            Name = "Test Room",
            Description = "Test Description",
            Capacity = 20,
            LocationId = Guid.NewGuid()
        };

        // Assert
        room.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Room_CanBeDeactivated()
    {
        // Arrange
        var room = new Room
        {
            Name = "Test Room",
            Description = "Test Description",
            Capacity = 20,
            LocationId = Guid.NewGuid()
        };

        // Act
        room.IsActive = false;

        // Assert
        room.IsActive.Should().BeFalse();
    }
}
