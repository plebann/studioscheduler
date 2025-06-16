using StudioScheduler.Core.Models;

namespace StudioScheduler.Core.Interfaces.Services;

public interface IPassService
{
    Task<Pass?> GetByIdAsync(Guid id);
    Task<IEnumerable<Pass>> GetAllAsync();
    Task<IEnumerable<Pass>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Pass>> GetActivePassesAsync();
    Task<IEnumerable<Pass>> GetExpiredPassesAsync();
    Task<Pass?> GetCurrentActivePassForUserAsync(Guid userId);
    Task<IEnumerable<Pass>> GetPassesExpiringInDaysAsync(int days);
    Task<Pass> CreatePassAsync(Pass pass);
    Task<Pass> UpdatePassAsync(Pass pass);
    Task DeletePassAsync(Guid id);
    Task<bool> CanUsePassForClassAsync(Guid passId, Guid scheduleId, DateTime classDate);
    Task<Pass> ExtendPassAsync(Guid passId, int additionalDays);
    Task<Pass> ActivatePassAsync(Guid passId);
    Task<Pass> DeactivatePassAsync(Guid passId);
    Task<IEnumerable<Pass>> GetPassesByTypeAsync(string passType);
    Task<int> GetPassUsageStatsAsync(Guid passId, DateTime? fromDate = null);
    Task<bool> IsPassValidForDateAsync(Guid passId, DateTime date);
}
