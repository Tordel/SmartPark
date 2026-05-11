using System.Net.Http.Json;
using SmartPark.Frontend.Models;

namespace SmartPark.Frontend.Services;

public class AiDetectionService
{
    private readonly HttpClient _httpClient;
    
    public AiDetectionService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AiDetectionResultDto?> AnalyzeImageAsync(AiDetectionRequestDto request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/ai/occupancy/analyze", request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AiDetectionResultDto>();
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error analyzing image: {ex.Message}");
            return null;
        }
    }
}