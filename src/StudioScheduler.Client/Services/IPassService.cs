using StudioScheduler.Shared.Dtos;

namespace StudioScheduler.Client.Services;

public interface IPassService
{
    Task<List<object>> GetMonthlyPassTypesAsync();
    Task<PassPurchaseResponseDto> PurchasePassAsync(BuyPassRequestDto request);
    Task<PassDto?> GetCurrentActivePassAsync(Guid userId);
}
