using System.Text.Json;
using StudioScheduler.Core.Models;
using StudioScheduler.Core.Interfaces.Repositories;

namespace StudioScheduler.Infrastructure.MockRepositories.Implementations;

public class MockRoomRepository : IRoomRepository
{
    private readonly string _dataPath;
    private List<Room> _rooms = new();

    public MockRoomRepository()
    {
        // Use relative path from server directory
        var currentDirectory = Directory.GetCurrentDirectory();
        _dataPath = Path.Combine(currentDirectory, "..", "StudioScheduler.Infrastructure", "MockRepositories", "Data", "rooms.json");
        
        LoadData();
    }

    private void LoadData()
    {
        if (!File.Exists(_dataPath))
        {
            Console.WriteLine($"Warning: Mock data file not found at {_dataPath}");
            _rooms = new List<Room>();
            return;
        }

        try
        {
            var json = File.ReadAllText(_dataPath);
            var data = JsonSerializer.Deserialize<RoomsData>(json, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });
            _rooms = data?.Rooms ?? new List<Room>();
            Console.WriteLine($"Loaded {_rooms.Count} rooms from mock data");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading mock data: {ex.Message}");
            _rooms = new List<Room>();
        }
    }

    public async Task<IEnumerable<Room>> GetAllAsync()
    {
        return await Task.FromResult(_rooms);
    }

    public async Task<Room?> GetByIdAsync(Guid id)
    {
        return await Task.FromResult(_rooms.FirstOrDefault(r => r.Id == id));
    }

    public async Task<Room> AddAsync(Room room)
    {
        _rooms.Add(room);
        return await Task.FromResult(room);
    }

    public async Task<Room> UpdateAsync(Room room)
    {
        var existing = _rooms.FindIndex(r => r.Id == room.Id);
        if (existing == -1)
            throw new KeyNotFoundException($"Room with id {room.Id} not found");

        _rooms[existing] = room;
        return await Task.FromResult(room);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var index = _rooms.FindIndex(r => r.Id == id);
        if (index == -1)
            return await Task.FromResult(false);

        _rooms.RemoveAt(index);
        return await Task.FromResult(true);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await Task.FromResult(_rooms.Any(r => r.Id == id));
    }

    public async Task<IEnumerable<Room>> GetByLocationIdAsync(Guid locationId)
    {
        return await Task.FromResult(_rooms.Where(r => r.LocationId == locationId));
    }

    public async Task<Room?> GetByLocationAndNameAsync(Guid locationId, string name)
    {
        return await Task.FromResult(_rooms.FirstOrDefault(r => 
            r.LocationId == locationId && 
            r.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
    }

    public async Task<IEnumerable<DanceClass>> GetClassesAsync(Guid roomId)
    {
        // In mock implementation, we don't track class relationships
        return await Task.FromResult(new List<DanceClass>());
    }

    public async Task<IEnumerable<Room>> GetByLocationAsync(Guid locationId)
    {
        return await GetByLocationIdAsync(locationId);
    }

    public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime startTime, TimeSpan duration)
    {
        // In mock implementation, we assume all rooms are available
        return await Task.FromResult(_rooms.Where(r => r.IsActive));
    }

    public async Task<bool> IsAvailableAsync(Guid roomId, DateTime startTime, TimeSpan duration)
    {
        // In mock implementation, we assume the room is available if it exists and is active
        var room = await GetByIdAsync(roomId);
        return room?.IsActive ?? false;
    }

    public Task SaveChangesAsync()
    {
        try
        {
            var data = new RoomsData { Rooms = _rooms };
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            return File.WriteAllTextAsync(_dataPath, json);
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }

    private class RoomsData
    {
        public List<Room> Rooms { get; set; } = new();
    }
}
