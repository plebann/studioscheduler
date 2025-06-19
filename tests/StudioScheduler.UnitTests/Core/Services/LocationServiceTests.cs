using FluentAssertions;
using Moq;
using StudioScheduler.Core.Interfaces.Repositories;
using StudioScheduler.Core.Models;
using StudioScheduler.Core.Services;

namespace StudioScheduler.UnitTests.Core.Services;

public class LocationServiceTests
{
    private readonly Mock<ILocationRepository> _mockLocationRepository;
    private readonly LocationService _locationService;

    public LocationServiceTests()
    {
        _mockLocationRepository = new Mock<ILocationRepository>();
        _locationService = new LocationService(_mockLocationRepository.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnLocation_WhenLocationExists()
    {
        // Arrange
        var locationId = Guid.NewGuid();
        var expectedLocation = new Location
        {
            Id = locationId,
            Name = "Main Studio",
            Address = "123 Dance Street",
            Description = "Main dance studio location",
            Capacity = 100,
            OpeningTime = new TimeSpan(8, 0, 0),
            ClosingTime = new TimeSpan(22, 0, 0)
        };

        _mockLocationRepository
            .Setup(x => x.GetByIdAsync(locationId))
            .ReturnsAsync(expectedLocation);

        // Act
        var result = await _locationService.GetByIdAsync(locationId);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(expectedLocation);
        _mockLocationRepository.Verify(x => x.GetByIdAsync(locationId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenLocationDoesNotExist()
    {
        // Arrange
        var locationId = Guid.NewGuid();
        _mockLocationRepository
            .Setup(x => x.GetByIdAsync(locationId))
            .ReturnsAsync((Location?)null);

        // Act
        var result = await _locationService.GetByIdAsync(locationId);

        // Assert
        result.Should().BeNull();
        _mockLocationRepository.Verify(x => x.GetByIdAsync(locationId), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllLocations()
    {
        // Arrange
        var locations = new List<Location>
        {
            new()
            {
                Name = "Studio 1",
                Address = "Address 1",
                Description = "Description 1",
                Capacity = 50,
                OpeningTime = new TimeSpan(8, 0, 0),
                ClosingTime = new TimeSpan(22, 0, 0)
            },
            new()
            {
                Name = "Studio 2",
                Address = "Address 2",
                Description = "Description 2",
                Capacity = 75,
                OpeningTime = new TimeSpan(9, 0, 0),
                ClosingTime = new TimeSpan(21, 0, 0)
            }
        };

        _mockLocationRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(locations);

        // Act
        var result = await _locationService.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(locations);
        _mockLocationRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateLocationAndSaveChanges()
    {
        // Arrange
        var location = new Location
        {
            Name = "New Studio",
            Address = "456 New Street",
            Description = "New studio location",
            Capacity = 80,
            OpeningTime = new TimeSpan(8, 0, 0),
            ClosingTime = new TimeSpan(22, 0, 0)
        };

        _mockLocationRepository
            .Setup(x => x.AddAsync(location))
            .ReturnsAsync(location);

        _mockLocationRepository
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _locationService.CreateAsync(location);

        // Assert
        result.Should().Be(location);
        _mockLocationRepository.Verify(x => x.AddAsync(location), Times.Once);
        _mockLocationRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateLocationAndSaveChanges()
    {
        // Arrange
        var location = new Location
        {
            Id = Guid.NewGuid(),
            Name = "Updated Studio",
            Address = "789 Updated Street",
            Description = "Updated studio location",
            Capacity = 90,
            OpeningTime = new TimeSpan(8, 0, 0),
            ClosingTime = new TimeSpan(22, 0, 0)
        };

        _mockLocationRepository
            .Setup(x => x.UpdateAsync(location))
            .ReturnsAsync(location);

        _mockLocationRepository
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _locationService.UpdateAsync(location);

        // Assert
        result.Should().Be(location);
        _mockLocationRepository.Verify(x => x.UpdateAsync(location), Times.Once);
        _mockLocationRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteLocationAndSaveChanges_WhenLocationExists()
    {
        // Arrange
        var locationId = Guid.NewGuid();
        _mockLocationRepository
            .Setup(x => x.DeleteAsync(locationId))
            .ReturnsAsync(true);

        _mockLocationRepository
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _locationService.DeleteAsync(locationId);

        // Assert
        result.Should().BeTrue();
        _mockLocationRepository.Verify(x => x.DeleteAsync(locationId), Times.Once);
        _mockLocationRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalseAndNotSaveChanges_WhenLocationDoesNotExist()
    {
        // Arrange
        var locationId = Guid.NewGuid();
        _mockLocationRepository
            .Setup(x => x.DeleteAsync(locationId))
            .ReturnsAsync(false);

        // Act
        var result = await _locationService.DeleteAsync(locationId);

        // Assert
        result.Should().BeFalse();
        _mockLocationRepository.Verify(x => x.DeleteAsync(locationId), Times.Once);
        _mockLocationRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenLocationExists()
    {
        // Arrange
        var locationId = Guid.NewGuid();
        _mockLocationRepository
            .Setup(x => x.ExistsAsync(locationId))
            .ReturnsAsync(true);

        // Act
        var result = await _locationService.ExistsAsync(locationId);

        // Assert
        result.Should().BeTrue();
        _mockLocationRepository.Verify(x => x.ExistsAsync(locationId), Times.Once);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenLocationDoesNotExist()
    {
        // Arrange
        var locationId = Guid.NewGuid();
        _mockLocationRepository
            .Setup(x => x.ExistsAsync(locationId))
            .ReturnsAsync(false);

        // Act
        var result = await _locationService.ExistsAsync(locationId);

        // Assert
        result.Should().BeFalse();
        _mockLocationRepository.Verify(x => x.ExistsAsync(locationId), Times.Once);
    }

    [Fact]
    public async Task GetLocationRoomsAsync_ShouldReturnRooms()
    {
        // Arrange
        var locationId = Guid.NewGuid();
        var rooms = new List<Room>
        {
            new()
            {
                Name = "Room 1",
                Description = "First room",
                Capacity = 30,
                LocationId = locationId
            },
            new()
            {
                Name = "Room 2",
                Description = "Second room",
                Capacity = 40,
                LocationId = locationId
            }
        };

        _mockLocationRepository
            .Setup(x => x.GetRoomsAsync(locationId))
            .ReturnsAsync(rooms);

        // Act
        var result = await _locationService.GetLocationRoomsAsync(locationId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(rooms);
        _mockLocationRepository.Verify(x => x.GetRoomsAsync(locationId), Times.Once);
    }

    [Fact]
    public async Task GetLocationSchedulesAsync_ShouldReturnSchedules()
    {
        // Arrange
        var locationId = Guid.NewGuid();
        var schedules = new List<Schedule>
        {
            new()
            {
                Name = "Monday Schedule",
                LocationId = locationId,
                EffectiveFrom = DateTime.UtcNow,
                DanceClassId = Guid.NewGuid(),
                DayOfWeek = DayOfWeek.Monday,
                StartTime = TimeSpan.FromHours(19), // 7 PM
                Duration = 60,
                Level = "Beginner",
                Capacity = 20
            }
        };

        _mockLocationRepository
            .Setup(x => x.GetSchedulesAsync(locationId))
            .ReturnsAsync(schedules);

        // Act
        var result = await _locationService.GetLocationSchedulesAsync(locationId);

        // Assert
        result.Should().HaveCount(1);
        result.Should().BeEquivalentTo(schedules);
        _mockLocationRepository.Verify(x => x.GetSchedulesAsync(locationId), Times.Once);
    }
}
