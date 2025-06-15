using StudioScheduler.Core.Interfaces.Repositories;
using StudioScheduler.Core.Interfaces.Services;
using StudioScheduler.Core.Models;

namespace StudioScheduler.Infrastructure.Services;

public class DanceClassService : IDanceClassService
{
    private readonly IDanceClassRepository _repository;
    private readonly IScheduleRepository _scheduleRepository;

    public DanceClassService(IDanceClassRepository repository, IScheduleRepository scheduleRepository)
    {
        _repository = repository;
        _scheduleRepository = scheduleRepository;
    }

    public async Task<DanceClass?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<DanceClass>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<IEnumerable<DanceClass>> GetByStyleAsync(string style)
    {
        return await _repository.GetByStyleAsync(style);
    }

    public async Task<IEnumerable<DanceClass>> GetByLevelAsync(string level)
    {
        return await _repository.GetByLevelAsync(level);
    }

    public async Task<IEnumerable<DanceClass>> GetByInstructorAsync(Guid instructorId)
    {
        return await _repository.GetByInstructorAsync(instructorId);
    }

    public async Task<DanceClass> CreateAsync(DanceClass danceClass)
    {
        var created = await _repository.AddAsync(danceClass);
        await _repository.SaveChangesAsync();
        return created;
    }

    public async Task<DanceClass> UpdateAsync(DanceClass danceClass)
    {
        var updated = await _repository.UpdateAsync(danceClass);
        await _repository.SaveChangesAsync();
        return updated;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var result = await _repository.DeleteAsync(id);
        if (result)
        {
            await _repository.SaveChangesAsync();
        }
        return result;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _repository.ExistsAsync(id);
    }

    public async Task<IEnumerable<Schedule>> GetClassSchedulesAsync(Guid classId)
    {
        return await _scheduleRepository.GetByDanceClassAsync(classId);
    }

    public async Task<IEnumerable<DanceClass>> GetClassesByRoomAsync(Guid roomId)
    {
        return await _repository.GetByRoomAsync(roomId);
    }

    public async Task<bool> IsInstructorAvailableAsync(Guid instructorId, DateTime startTime, TimeSpan duration)
    {
        return await _repository.IsInstructorAvailableAsync(instructorId, startTime, duration);
    }

    public async Task<int> GetCurrentEnrollmentAsync(Guid classId)
    {
        return await _repository.GetCurrentEnrollmentAsync(classId);
    }
}
