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
}
