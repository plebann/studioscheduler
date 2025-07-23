using StudioScheduler.Core.Models;

namespace StudioScheduler.Core.Interfaces.Services;

public interface IDanceClassService
{
    Task<DanceClass?> GetByIdAsync(Guid id);
    Task<IEnumerable<DanceClass>> GetAllAsync();
    Task<DanceClass> CreateAsync(DanceClass danceClass);
    Task<DanceClass> UpdateAsync(DanceClass danceClass);
    Task<bool> DeleteAsync(Guid id);
}
