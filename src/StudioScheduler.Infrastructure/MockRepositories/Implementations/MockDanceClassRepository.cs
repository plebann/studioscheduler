using System.Text.Json;
using StudioScheduler.Core.Models;
using StudioScheduler.Core.Interfaces.Repositories;
using System.Reflection;

namespace StudioScheduler.Infrastructure.MockRepositories.Implementations;

public class MockDanceClassRepository : IDanceClassRepository
{
    private readonly string _dataPath;
    private List<DanceClass> _classes = new();

    public MockDanceClassRepository()
    {
        // Use relative path from server directory
        var currentDirectory = Directory.GetCurrentDirectory();
        _dataPath = Path.Combine(currentDirectory, "..", "StudioScheduler.Infrastructure", "MockRepositories", "Data", "classes.json");
        
        LoadData();
    }

    private void LoadData()
    {
        if (!File.Exists(_dataPath))
        {
            Console.WriteLine($"Warning: Mock data file not found at {_dataPath}");
            _classes = new List<DanceClass>();
            return;
        }

        try
        {
            var json = File.ReadAllText(_dataPath);
            var data = JsonSerializer.Deserialize<ClassesData>(json, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });
            _classes = data?.Classes ?? new List<DanceClass>();
            Console.WriteLine($"Loaded {_classes.Count} dance classes from mock data");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading mock data: {ex.Message}");
            _classes = new List<DanceClass>();
        }
    }

    public async Task<IEnumerable<DanceClass>> GetAllAsync()
    {
        return await Task.FromResult(_classes);
    }

    public async Task<DanceClass?> GetByIdAsync(Guid id)
    {
        return await Task.FromResult(_classes.FirstOrDefault(c => c.Id == id));
    }

    public async Task<DanceClass> AddAsync(DanceClass danceClass)
    {
        _classes.Add(danceClass);
        return await Task.FromResult(danceClass);
    }

    public async Task<DanceClass> CreateAsync(DanceClass danceClass)
    {
        return await AddAsync(danceClass);
    }

    public async Task<DanceClass> UpdateAsync(DanceClass danceClass)
    {
        var existing = _classes.FindIndex(c => c.Id == danceClass.Id);
        if (existing == -1)
            throw new KeyNotFoundException($"DanceClass with id {danceClass.Id} not found");

        _classes[existing] = danceClass;
        return await Task.FromResult(danceClass);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var index = _classes.FindIndex(c => c.Id == id);
        if (index == -1)
            return await Task.FromResult(false);

        _classes.RemoveAt(index);
        return await Task.FromResult(true);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await Task.FromResult(_classes.Any(c => c.Id == id));
    }

    public async Task<IEnumerable<DanceClass>> GetByStyleAsync(string style)
    {
        return await Task.FromResult(_classes.Where(c => c.Style.Equals(style, StringComparison.OrdinalIgnoreCase)));
    }

    public async Task<IEnumerable<DanceClass>> GetByLevelAsync(string level)
    {
        return await Task.FromResult(_classes.Where(c => c.Level.Equals(level, StringComparison.OrdinalIgnoreCase)));
    }

    public async Task<IEnumerable<DanceClass>> GetByInstructorAsync(Guid instructorId)
    {
        return await Task.FromResult(_classes.Where(c => c.InstructorId == instructorId));
    }

    public async Task<IEnumerable<DanceClass>> GetByRoomAsync(Guid roomId)
    {
        return await Task.FromResult(_classes.Where(c => c.RoomId == roomId));
    }

    public async Task<IEnumerable<Schedule>> GetSchedulesAsync(Guid classId)
    {
        // In mock implementation, we don't have schedule relationships
        return await Task.FromResult(new List<Schedule>());
    }

    public async Task<int> GetCurrentEnrollmentAsync(Guid classId)
    {
        // In mock implementation, we don't track enrollment
        return await Task.FromResult(0);
    }

    public async Task<bool> IsInstructorAvailableAsync(Guid instructorId, DateTime time, TimeSpan duration)
    {
        // In mock implementation, we assume instructors are always available
        return await Task.FromResult(true);
    }

    public Task SaveChangesAsync()
    {
        try
        {
            var data = new ClassesData { Classes = _classes };
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            return File.WriteAllTextAsync(_dataPath, json);
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }

    private class ClassesData
    {
        public List<DanceClass> Classes { get; set; } = new();
    }
}
