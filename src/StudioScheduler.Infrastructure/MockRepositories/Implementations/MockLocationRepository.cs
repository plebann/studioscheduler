using System.Text.Json;
using StudioScheduler.Core.Models;
using StudioScheduler.Core.Interfaces.Repositories;

namespace StudioScheduler.Infrastructure.MockRepositories.Implementations;

public class MockLocationRepository : ILocationRepository
{
    private readonly string _dataPath;
    private List<Location> _locations = new();

    public MockLocationRepository()
    {
        // Use relative path from server directory
        var currentDirectory = Directory.GetCurrentDirectory();
        _dataPath = Path.Combine(currentDirectory, "..", "StudioScheduler.Infrastructure", "MockRepositories", "Data", "locations.json");
        
        LoadData();
    }

    private void LoadData()
    {
        if (!File.Exists(_dataPath))
        {
            Console.WriteLine($"Warning: Mock data file not found at {_dataPath}");
            _locations = new List<Location>();
            return;
        }

        try
        {
            var json = File.ReadAllText(_dataPath);
            var data = JsonSerializer.Deserialize<LocationsData>(json, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });
            _locations = data?.Locations ?? new List<Location>();
            Console.WriteLine($"Loaded {_locations.Count} locations from mock data");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading mock data: {ex.Message}");
            _locations = new List<Location>();
        }
    }

    public async Task<IEnumerable<Location>> GetAllAsync()
    {
        return await Task.FromResult(_locations);
    }

    public async Task<Location?> GetByIdAsync(Guid id)
    {
        return await Task.FromResult(_locations.FirstOrDefault(l => l.Id == id));
    }

    public async Task<Location> AddAsync(Location location)
    {
        _locations.Add(location);
        return await Task.FromResult(location);
    }

    public async Task<Location> UpdateAsync(Location location)
    {
        var existing = _locations.FindIndex(l => l.Id == location.Id);
        if (existing == -1)
            throw new KeyNotFoundException($"Location with id {location.Id} not found");

        _locations[existing] = location;
        return await Task.FromResult(location);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var index = _locations.FindIndex(l => l.Id == id);
        if (index == -1)
            return await Task.FromResult(false);

        _locations.RemoveAt(index);
        return await Task.FromResult(true);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await Task.FromResult(_locations.Any(l => l.Id == id));
    }

    public async Task<Location?> GetByNameAsync(string name)
    {
        return await Task.FromResult(_locations.FirstOrDefault(l => 
            l.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
    }

    public async Task<IEnumerable<Room>> GetRoomsAsync(Guid locationId)
    {
        // In mock implementation, we don't track room relationships
        return await Task.FromResult(new List<Room>());
    }

    public async Task<IEnumerable<DanceClass>> GetClassesAsync(Guid locationId)
    {
        // In mock implementation, we don't track class relationships
        return await Task.FromResult(new List<DanceClass>());
    }

    public async Task<IEnumerable<Schedule>> GetSchedulesAsync(Guid locationId)
    {
        // In mock implementation, we don't track schedule relationships
        return await Task.FromResult(new List<Schedule>());
    }

    public Task SaveChangesAsync()
    {
        try
        {
            var data = new LocationsData { Locations = _locations };
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            return File.WriteAllTextAsync(_dataPath, json);
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }

    private class LocationsData
    {
        public List<Location> Locations { get; set; } = new();
    }
}
