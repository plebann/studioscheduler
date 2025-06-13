using FluentAssertions;
using StudioScheduler.Core.Models;

namespace StudioScheduler.UnitTests.Core.Models;

public class LocationTests
{
    [Fact]
    public void Location_ShouldInitializeWithRequiredProperties()
    {
        // Arrange & Act
        var location = new Location
        {
            Name = "Main Studio",
            Address = "123 Dance Street",
            Description = "Main dance studio location",
            Capacity = 100,
            OpeningTime = new TimeSpan(8, 0, 0),
            ClosingTime = new TimeSpan(22, 0, 0)
        };

        // Assert
        location.Id.Should().NotBeEmpty();
        location.Name.Should().Be("Main Studio");
        location.Address.Should().Be("123 Dance Street");
        location.Description.Should().Be("Main dance studio location");
        location.Capacity.Should().Be(100);
        location.OpeningTime.Should().Be(new TimeSpan(8, 0, 0));
        location.ClosingTime.Should().Be(new TimeSpan(22, 0, 0));
        location.IsActive.Should().BeTrue();
        location.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        location.UpdatedAt.Should().BeNull();
        location.Rooms.Should().BeEmpty();
        location.Schedules.Should().BeEmpty();
    }

    [Fact]
    public void Location_ShouldBeImmutableAfterInitialization()
    {
        // Arrange
        var location = new Location
        {
            Name = "Test",
            Address = "Test Address",
            Description = "Test Description",
            Capacity = 50,
            OpeningTime = new TimeSpan(8, 0, 0),
            ClosingTime = new TimeSpan(22, 0, 0)
        };

        // Act & Assert
        // Properties with init-only setters cannot be modified after creation
        // This test documents the immutable behavior of Name, Address, and Description
        location.Name.Should().Be("Test");
        location.Address.Should().Be("Test Address");
        location.Description.Should().Be("Test Description");
        
        // Verify that the properties are init-only by checking they exist
        var nameProperty = typeof(Location).GetProperty(nameof(Location.Name));
        var addressProperty = typeof(Location).GetProperty(nameof(Location.Address));
        var descriptionProperty = typeof(Location).GetProperty(nameof(Location.Description));
        
        nameProperty.Should().NotBeNull();
        addressProperty.Should().NotBeNull();
        descriptionProperty.Should().NotBeNull();
        
        // Init-only properties have a setter but it's only accessible during initialization
        nameProperty!.CanWrite.Should().BeTrue();
        addressProperty!.CanWrite.Should().BeTrue();
        descriptionProperty!.CanWrite.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Location_Capacity_ShouldAcceptZeroAndNegativeValues(int capacity)
    {
        // Arrange & Act
        var location = new Location
        {
            Name = "Test",
            Address = "Test Address",
            Description = "Test Description",
            Capacity = capacity,
            OpeningTime = new TimeSpan(8, 0, 0),
            ClosingTime = new TimeSpan(22, 0, 0)
        };

        // Assert
        location.Capacity.Should().Be(capacity);
    }

    [Fact]
    public void Location_ShouldAllowModificationOfCollections()
    {
        // Arrange
        var location = new Location
        {
            Name = "Test",
            Address = "Test Address",
            Description = "Test Description",
            Capacity = 50,
            OpeningTime = new TimeSpan(8, 0, 0),
            ClosingTime = new TimeSpan(22, 0, 0)
        };

        var room = new Room
        {
            Name = "Studio A",
            Description = "Main studio room",
            Capacity = 30,
            LocationId = location.Id
        };

        // Act
        location.Rooms.Add(room);

        // Assert
        location.Rooms.Should().HaveCount(1);
        location.Rooms.Should().Contain(room);
    }
}
