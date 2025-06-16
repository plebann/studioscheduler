using StudioScheduler.Core.Models;

namespace StudioScheduler.Core.Interfaces.Repositories;

public interface IPassRepository
{
    Task<Pass?> GetByIdAsync(Guid id);
    Task<IEnumerable<Pass>> GetAllAsync();
    Task<IEnumerable<Pass>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Pass>> GetActivePassesAsync();
    Task<IEnumerable<Pass>> GetExpiredPassesAsync();
    Task<IEnumerable<Pass>> GetPassesByTypeAsync(string passType);
    Task<Pass?> GetCurrentActivePassForUserAsync(Guid userId);
    Task<IEnumerable<Pass>> GetPassesExpiringInDaysAsync(int days);
    Task<Pass> AddAsync(Pass pass);
    Task<Pass> UpdateAsync(Pass pass);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<int> CountAsync();
    Task<int> CountActiveAsync();
    Task<int> CountByTypeAsync(string passType);
}
