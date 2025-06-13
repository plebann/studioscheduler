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
    public void Location_Name_ShouldThrowArgumentNullException_WhenSetToNull()
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
        var act = () => location.Name = null!;
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("Name");
    }

    [Fact]
    public void Location_Address_ShouldThrowArgumentNullException_WhenSetToNull()
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
        var act = () => location.Address = null!;
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("Address");
    }

    [Fact]
    public void Location_Description_ShouldThrowArgumentNullException_WhenSetToNull()
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
        var act = () => location.Description = null!;
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("Description");
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
