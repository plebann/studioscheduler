using System.Text.Json;
using StudioScheduler.Core.Models;
using StudioScheduler.Core.Interfaces.Repositories;

namespace StudioScheduler.Infrastructure.MockRepositories.Implementations;

public class MockScheduleRepository : IScheduleRepository
{
    private readonly string _dataPath;
    private List<Schedule> _schedules = new();

    public MockScheduleRepository()
    {
        // Use relative path from server directory
        var currentDirectory = Directory.GetCurrentDirectory();
        _dataPath = Path.Combine(currentDirectory, "..", "StudioScheduler.Infrastructure", "MockRepositories", "Data", "schedules.json");
        
        LoadData();
    }

    private void LoadData()
    {
        if (!File.Exists(_dataPath))
        {
            Console.WriteLine($"Warning: Mock data file not found at {_dataPath}");
            _schedules = new List<Schedule>();
            return;
        }

        try
        {
            var json = File.ReadAllText(_dataPath);
            var data = JsonSerializer.Deserialize<SchedulesData>(json, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });
            _schedules = data?.Schedules ?? new List<Schedule>();
            Console.WriteLine($"Loaded {_schedules.Count} schedules from mock data");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading mock data: {ex.Message}");
            _schedules = new List<Schedule>();
        }
    }

    public async Task<IEnumerable<Schedule>> GetAllAsync()
    {
        return await Task.FromResult(_schedules);
    }

    public async Task<IEnumerable<Schedule>> GetActiveSchedulesAsync()
    {
        return await Task.FromResult(_schedules.Where(s => 
            s.IsActive && !s.IsCancelled && 
            (s.EffectiveTo == null || s.EffectiveTo > DateTime.UtcNow)));
    }

    public async Task<Schedule?> GetByIdAsync(Guid id)
    {
        return await Task.FromResult(_schedules.FirstOrDefault(s => s.Id == id));
    }

    public async Task<Schedule> AddAsync(Schedule schedule)
    {
        _schedules.Add(schedule);
        return await Task.FromResult(schedule);
    }

    public async Task<Schedule> UpdateAsync(Schedule schedule)
    {
        var existing = _schedules.FindIndex(s => s.Id == schedule.Id);
        if (existing == -1)
            throw new KeyNotFoundException($"Schedule with id {schedule.Id} not found");

        _schedules[existing] = schedule;
        return await Task.FromResult(schedule);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var index = _schedules.FindIndex(s => s.Id == id);
        if (index == -1)
            return await Task.FromResult(false);

        _schedules.RemoveAt(index);
        return await Task.FromResult(true);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await Task.FromResult(_schedules.Any(s => s.Id == id));
    }

    public async Task<IEnumerable<Schedule>> GetByDateRangeAsync(DateTime start, DateTime end)
    {
        return await Task.FromResult(_schedules.Where(s => 
            s.StartTime >= start && 
            s.StartTime <= end));
    }

    public async Task<IEnumerable<Schedule>> GetByDayOfWeekAsync(DayOfWeek dayOfWeek)
    {
        return await Task.FromResult(_schedules.Where(s => 
            s.StartTime.DayOfWeek == dayOfWeek));
    }

    public async Task<IEnumerable<Schedule>> GetByLocationAsync(Guid locationId)
    {
        return await Task.FromResult(_schedules.Where(s => 
            s.LocationId == locationId));
    }

    public async Task<IEnumerable<Schedule>> GetByRoomAsync(Guid roomId)
    {
        // In this mock, we don't track room assignments directly
        return await Task.FromResult(new List<Schedule>());
    }

    public async Task<IEnumerable<Schedule>> GetByInstructorAsync(Guid instructorId)
    {
        return await Task.FromResult(_schedules.Where(s => 
            s.DanceClass?.InstructorId == instructorId));
    }

    public async Task<IEnumerable<Schedule>> GetByDanceClassAsync(Guid classId)
    {
        return await Task.FromResult(_schedules.Where(s => 
            s.DanceClassId == classId));
    }

    public async Task<bool> IsTimeSlotAvailableAsync(Guid locationId, DateTime startTime, TimeSpan duration)
    {
        var endTime = startTime.Add(duration);
        return await Task.FromResult(!_schedules.Any(s =>
            s.LocationId == locationId &&
            s.StartTime < endTime &&
            s.StartTime.Add(s.Duration) > startTime));
    }

    public async Task<bool> HasScheduleConflictAsync(Guid locationId, DateTime startTime, TimeSpan duration, Guid? excludeScheduleId = null)
    {
        var endTime = startTime.Add(duration);
        return await Task.FromResult(_schedules.Any(s =>
            s.LocationId == locationId &&
            s.Id != excludeScheduleId &&
            s.StartTime < endTime &&
            s.StartTime.Add(s.Duration) > startTime));
    }

    public async Task<bool> CancelScheduleAsync(Guid id)
    {
        var schedule = await GetByIdAsync(id);
        if (schedule == null)
            return false;

        schedule.IsCancelled = true;
        schedule.UpdatedAt = DateTime.UtcNow;
        return true;
    }

    public async Task<int> GetAvailableSpotsAsync(Guid scheduleId)
    {
        var schedule = await GetByIdAsync(scheduleId);
        if (schedule == null || schedule.DanceClass == null)
            return 0;

        var capacity = schedule.DanceClass.Capacity;
        var reservationCount = schedule.Reservations.Count;
        return Math.Max(0, capacity - reservationCount);
    }

    public Task SaveChangesAsync()
    {
        try
        {
            var data = new SchedulesData { Schedules = _schedules };
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            return File.WriteAllTextAsync(_dataPath, json);
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }

    private class SchedulesData
    {
        public List<Schedule> Schedules { get; set; } = new();
    }
}
