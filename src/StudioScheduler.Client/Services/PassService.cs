using System.Net.Http.Json;
using StudioScheduler.Shared.Dtos;
using StudioScheduler.Core.Enums;

namespace StudioScheduler.Client.Services;

public interface IPassService
{
    Task<List<object>> GetMonthlyPassTypesAsync();
    Task<PassPurchaseResponseDto> PurchasePassAsync(BuyPassRequestDto request);
    Task<PassDto?> GetCurrentActivePassAsync(Guid userId);
}

public class PassService : IPassService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PassService> _logger;

    public PassService(HttpClient httpClient, ILogger<PassService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<object>> GetMonthlyPassTypesAsync()
    {
        try
        {
            _logger.LogInformation("Fetching monthly pass types from API");
            var response = await _httpClient.GetFromJsonAsync<List<object>>("api/pass/types/monthly");
            return response ?? new List<object>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching monthly pass types");
            return new List<object>();
        }
    }

    public async Task<PassPurchaseResponseDto> PurchasePassAsync(BuyPassRequestDto request)
    {
        try
        {
            _logger.LogInformation("Purchasing pass for student {StudentId}", request.StudentId);
            var response = await _httpClient.PostAsJsonAsync("api/pass/purchase", request);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PassPurchaseResponseDto>();
                return result ?? new PassPurchaseResponseDto { Success = false, ErrorMessage = "Invalid response" };
            }
            else
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<PassPurchaseResponseDto>();
                return errorResponse ?? new PassPurchaseResponseDto 
                { 
                    Success = false, 
                    ErrorMessage = $"HTTP {response.StatusCode}: {response.ReasonPhrase}" 
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error purchasing pass for student {StudentId}", request.StudentId);
            return new PassPurchaseResponseDto
            {
                Success = false,
                ErrorMessage = $"Error purchasing pass: {ex.Message}"
            };
        }
    }

    public async Task<PassDto?> GetCurrentActivePassAsync(Guid userId)
    {
        try
        {
            _logger.LogInformation("Fetching current active pass for user {UserId}", userId);
            var response = await _httpClient.GetFromJsonAsync<PassDto>($"api/pass/user/{userId}/current");
            
            if (response != null)
            {
                _logger.LogInformation("Found active pass {PassId} for user {UserId}, Status: {Status}, IsActive: {IsActive}", 
                    response.Id, userId, response.PassStatus, response.IsActive);
            }
            
            return response;
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("404"))
        {
            // No active pass found - this is expected for users without passes
            _logger.LogInformation("No active pass found for user {UserId}", userId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching current active pass for user {UserId}", userId);
            return null;
        }
    }
}
