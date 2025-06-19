using FluentAssertions;
using Moq;
using StudioScheduler.Core.Interfaces.Repositories;
using StudioScheduler.Core.Models;
using StudioScheduler.Core.Services;

namespace StudioScheduler.UnitTests.Core.Services;

public class RoomServiceTests
{
    private readonly Mock<IRoomRepository> _mockRoomRepository;
    private readonly Mock<ILocationRepository> _mockLocationRepository;
    private readonly RoomService _roomService;

    public RoomServiceTests()
    {
        _mockRoomRepository = new Mock<IRoomRepository>();
        _mockLocationRepository = new Mock<ILocationRepository>();
        _roomService = new RoomService(_mockRoomRepository.Object, _mockLocationRepository.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnRoom_WhenRoomExists()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var expectedRoom = new Room
        {
            Id = roomId,
            Name = "Studio A",
            Description = "Main dance studio",
            Capacity = 30,
            LocationId = Guid.NewGuid()
        };

        _mockRoomRepository
            .Setup(x => x.GetByIdAsync(roomId))
            .ReturnsAsync(expectedRoom);

        // Act
        var result = await _roomService.GetByIdAsync(roomId);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(expectedRoom);
        _mockRoomRepository.Verify(x => x.GetByIdAsync(roomId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenRoomDoesNotExist()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        _mockRoomRepository
            .Setup(x => x.GetByIdAsync(roomId))
            .ReturnsAsync((Room?)null);

        // Act
        var result = await _roomService.GetByIdAsync(roomId);

        // Assert
        result.Should().BeNull();
        _mockRoomRepository.Verify(x => x.GetByIdAsync(roomId), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllRooms()
    {
        // Arrange
        var rooms = new List<Room>
        {
            new()
            {
                Name = "Studio A",
                Description = "First studio",
                Capacity = 30,
                LocationId = Guid.NewGuid()
            },
            new()
            {
                Name = "Studio B",
                Description = "Second studio",
                Capacity = 25,
                LocationId = Guid.NewGuid()
            }
        };

        _mockRoomRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(rooms);

        // Act
        var result = await _roomService.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(rooms);
        _mockRoomRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateRoomAndSaveChanges_WhenLocationExistsAndNameIsUnique()
    {
        // Arrange
        var locationId = Guid.NewGuid();
        var room = new Room
        {
            Name = "New Studio",
            Description = "Brand new studio",
            Capacity = 35,
            LocationId = locationId
        };

        _mockLocationRepository
            .Setup(x => x.ExistsAsync(locationId))
            .ReturnsAsync(true);

        _mockRoomRepository
            .Setup(x => x.GetByLocationAndNameAsync(locationId, room.Name))
            .ReturnsAsync((Room?)null);

        _mockRoomRepository
            .Setup(x => x.AddAsync(room))
            .ReturnsAsync(room);

        _mockRoomRepository
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _roomService.CreateAsync(room);

        // Assert
        result.Should().Be(room);
        _mockLocationRepository.Verify(x => x.ExistsAsync(locationId), Times.Once);
        _mockRoomRepository.Verify(x => x.GetByLocationAndNameAsync(locationId, room.Name), Times.Once);
        _mockRoomRepository.Verify(x => x.AddAsync(room), Times.Once);
        _mockRoomRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowArgumentException_WhenLocationDoesNotExist()
    {
        // Arrange
        var locationId = Guid.NewGuid();
        var room = new Room
        {
            Name = "New Studio",
            Description = "Brand new studio",
            Capacity = 35,
            LocationId = locationId
        };

        _mockLocationRepository
            .Setup(x => x.ExistsAsync(locationId))
            .ReturnsAsync(false);

        // Act & Assert
        var act = async () => await _roomService.CreateAsync(room);
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("LocationId")
            .WithMessage("Location does not exist*");

        _mockLocationRepository.Verify(x => x.ExistsAsync(locationId), Times.Once);
        _mockRoomRepository.Verify(x => x.AddAsync(It.IsAny<Room>()), Times.Never);
        _mockRoomRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowInvalidOperationException_WhenRoomNameAlreadyExistsInLocation()
    {
        // Arrange
        var locationId = Guid.NewGuid();
        var room = new Room
        {
            Name = "Existing Studio",
            Description = "Studio with existing name",
            Capacity = 35,
            LocationId = locationId
        };

        var existingRoom = new Room
        {
            Id = Guid.NewGuid(),
            Name = "Existing Studio",
            Description = "Already existing studio",
            Capacity = 30,
            LocationId = locationId
        };

        _mockLocationRepository
            .Setup(x => x.ExistsAsync(locationId))
            .ReturnsAsync(true);

        _mockRoomRepository
            .Setup(x => x.GetByLocationAndNameAsync(locationId, room.Name))
            .ReturnsAsync(existingRoom);

        // Act & Assert
        var act = async () => await _roomService.CreateAsync(room);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Room with name 'Existing Studio' already exists in this location");

        _mockLocationRepository.Verify(x => x.ExistsAsync(locationId), Times.Once);
        _mockRoomRepository.Verify(x => x.GetByLocationAndNameAsync(locationId, room.Name), Times.Once);
        _mockRoomRepository.Verify(x => x.AddAsync(It.IsAny<Room>()), Times.Never);
        _mockRoomRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateRoomAndSaveChanges_WhenLocationExistsAndNameIsUniqueOrSame()
    {
        // Arrange
        var locationId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        var room = new Room
        {
            Id = roomId,
            Name = "Updated Studio",
            Description = "Updated studio description",
            Capacity = 40,
            LocationId = locationId
        };

        _mockLocationRepository
            .Setup(x => x.ExistsAsync(locationId))
            .ReturnsAsync(true);

        _mockRoomRepository
            .Setup(x => x.GetByLocationAndNameAsync(locationId, room.Name))
            .ReturnsAsync((Room?)null);

        _mockRoomRepository
            .Setup(x => x.UpdateAsync(room))
            .ReturnsAsync(room);

        _mockRoomRepository
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _roomService.UpdateAsync(room);

        // Assert
        result.Should().Be(room);
        _mockLocationRepository.Verify(x => x.ExistsAsync(locationId), Times.Once);
        _mockRoomRepository.Verify(x => x.GetByLocationAndNameAsync(locationId, room.Name), Times.Once);
        _mockRoomRepository.Verify(x => x.UpdateAsync(room), Times.Once);
        _mockRoomRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowArgumentException_WhenLocationDoesNotExist()
    {
        // Arrange
        var locationId = Guid.NewGuid();
        var room = new Room
        {
            Id = Guid.NewGuid(),
            Name = "Updated Studio",
            Description = "Updated studio description",
            Capacity = 40,
            LocationId = locationId
        };

        _mockLocationRepository
            .Setup(x => x.ExistsAsync(locationId))
            .ReturnsAsync(false);

        // Act & Assert
        var act = async () => await _roomService.UpdateAsync(room);
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("LocationId")
            .WithMessage("Location does not exist*");

        _mockLocationRepository.Verify(x => x.ExistsAsync(locationId), Times.Once);
        _mockRoomRepository.Verify(x => x.UpdateAsync(It.IsAny<Room>()), Times.Never);
        _mockRoomRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteRoomAndSaveChanges_WhenRoomExists()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        _mockRoomRepository
            .Setup(x => x.DeleteAsync(roomId))
            .ReturnsAsync(true);

        _mockRoomRepository
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _roomService.DeleteAsync(roomId);

        // Assert
        result.Should().BeTrue();
        _mockRoomRepository.Verify(x => x.DeleteAsync(roomId), Times.Once);
        _mockRoomRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalseAndNotSaveChanges_WhenRoomDoesNotExist()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        _mockRoomRepository
            .Setup(x => x.DeleteAsync(roomId))
            .ReturnsAsync(false);

        // Act
        var result = await _roomService.DeleteAsync(roomId);

        // Assert
        result.Should().BeFalse();
        _mockRoomRepository.Verify(x => x.DeleteAsync(roomId), Times.Once);
        _mockRoomRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenRoomExists()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        _mockRoomRepository
            .Setup(x => x.ExistsAsync(roomId))
            .ReturnsAsync(true);

        // Act
        var result = await _roomService.ExistsAsync(roomId);

        // Assert
        result.Should().BeTrue();
        _mockRoomRepository.Verify(x => x.ExistsAsync(roomId), Times.Once);
    }

    [Fact]
    public async Task IsRoomAvailableAsync_ShouldReturnAvailabilityStatus()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var startTime = DateTime.UtcNow.AddDays(1);
        var duration = TimeSpan.FromMinutes(60);

        _mockRoomRepository
            .Setup(x => x.IsAvailableAsync(roomId, DayOfWeek.Monday, startTime.TimeOfDay, duration))
            .ReturnsAsync(true);

        // Act
        var result = await _roomService.IsRoomAvailableAsync(roomId, DayOfWeek.Monday, startTime.TimeOfDay, duration);

        // Assert
        result.Should().BeTrue();
        _mockRoomRepository.Verify(x => x.IsAvailableAsync(roomId, DayOfWeek.Monday, startTime.TimeOfDay, duration), Times.Once);
    }

    [Fact]
    public async Task GetRoomSchedulesAsync_ShouldReturnSchedules()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var schedules = new List<Schedule>
        {
            new()
            {
                Name = "Monday Evening Salsa",
                LocationId = Guid.NewGuid(),
                EffectiveFrom = DateTime.UtcNow,
                DanceClassId = Guid.NewGuid(),
                DayOfWeek = DayOfWeek.Monday,
                StartTime = TimeSpan.FromHours(19), // 7 PM
                Duration = 60,
                Level = "Beginner",
                Capacity = 20
            }
        };

        _mockRoomRepository
            .Setup(x => x.GetSchedulesAsync(roomId))
            .ReturnsAsync(schedules);

        // Act
        var result = await _roomService.GetRoomSchedulesAsync(roomId);

        // Assert
        result.Should().HaveCount(1);
        result.Should().BeEquivalentTo(schedules);
        _mockRoomRepository.Verify(x => x.GetSchedulesAsync(roomId), Times.Once);
    }

    [Fact]
    public async Task GetRoomByLocationAndNameAsync_ShouldReturnRoom_WhenExists()
    {
        // Arrange
        var locationId = Guid.NewGuid();
        var roomName = "Studio A";
        var expectedRoom = new Room
        {
            Name = roomName,
            Description = "Main studio",
            Capacity = 30,
            LocationId = locationId
        };

        _mockRoomRepository
            .Setup(x => x.GetByLocationAndNameAsync(locationId, roomName))
            .ReturnsAsync(expectedRoom);

        // Act
        var result = await _roomService.GetRoomByLocationAndNameAsync(locationId, roomName);

        // Assert
        result.Should().Be(expectedRoom);
        _mockRoomRepository.Verify(x => x.GetByLocationAndNameAsync(locationId, roomName), Times.Once);
    }
}
