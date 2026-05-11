using System.Net.Http.Json;
using SmartPark.Frontend.Models;

namespace SmartPark.Frontend.Services;

public class ParkingSpaceService
{
    private readonly HttpClient _httpClient;

    public ParkingSpaceService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PaginatedParkingSpacesDto?> GetAllParkingSpacesAsync(int? pageNumber = null, int? pageSize = null)
    {
        try
        {
            var url = "api/parkingspace";
            var queryParams = new List<string>();

            if (pageNumber.HasValue)
                queryParams.Add($"pageNumber={pageNumber}");
            if (pageSize.HasValue)
                queryParams.Add($"pageSize={pageSize}");

            if (queryParams.Count > 0)
                url += "?" + string.Join("&", queryParams);

            return await _httpClient.GetFromJsonAsync<PaginatedParkingSpacesDto>(url);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching parking spaces: {ex.Message}");
            return null;
        }
    }

    public async Task<ParkingSpaceDto?> GetParkingSpaceByIdAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ParkingSpaceDto>($"api/parkingspace/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching parking space: {ex.Message}");
            return null;
        }
    }

    public async Task<ParkingSpaceHistoryDto?> GetParkingSpaceHistoryAsync(Guid id, DateTime start, DateTime end)
    {
        try
        {
            var url = $"api/parkingspace/{id}/history?start={start:yyyy-MM-ddTHH:mm:ssZ}&end={end:yyyy-MM-ddTHH:mm:ssZ}";
            return await _httpClient.GetFromJsonAsync<ParkingSpaceHistoryDto>(url);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching history: {ex.Message}");
            return null;
        }
    }

    public async Task<List<ParkingSpaceDto>?> GetAvailableParkingSpacesAsync(int? maxResults = null)
    {
        try
        {
            var url = "api/parkingspace/available";
            if (maxResults.HasValue)
                url += $"?maxResults={maxResults}";

            return await _httpClient.GetFromJsonAsync<List<ParkingSpaceDto>>(url);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching available spaces: {ex.Message}");
            return null;
        }
    }

    public async Task<OccupancyRateDto?> GetOccupancyRateAsync(DateTime start, DateTime end,
        Guid? parkingSpaceId = null)
    {
        try
        {
            var url = $"api/parkingspace/occupancy" +
                      $"?start={start:yyyy-MM-ddTHH:mm:ssZ}" +
                      $"&end={end:yyyy-MM-ddTHH:mm:ssZ}";
            if (parkingSpaceId.HasValue)
                url += $"&parkingSpaceId={parkingSpaceId}";

            return await _httpClient.GetFromJsonAsync<OccupancyRateDto>(url);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching occupancy rate: {ex.Message}");
            return null;
        }
    }

    public async Task<List<ParkingSpaceDto>?> GetParkingSpacesByStatusAsync(string status, int? maxResults = null)
    {
        try
        {
            var url = $"api/parkingspace/by-status/{status}";
            if (maxResults.HasValue)
                url += $"?maxResults={maxResults}";

            return await _httpClient.GetFromJsonAsync<List<ParkingSpaceDto>>(url);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching parking spaces by status: {ex.Message}");
            return null;
        }   
    }
    
    public async Task<bool> CreateParkingSpaceAsync(string spaceNumber, decimal hourlyRate, string type)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/parkingspace", new
            {
                SpaceNumber = spaceNumber,
                HourlyRate = hourlyRate,
                Type = type
            });
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating parking space: {ex.Message}");
            return false;
        }
    }
 
    public async Task<bool> DeleteParkingSpaceAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/parkingspace/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting parking space: {ex.Message}");
            return false;
        }
    }
 
    public async Task<bool> UpdateParkingSpaceStatusAsync(Guid id, string status)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/parkingspace/{id}/status", new
            {
                ParkingSpaceId = id,
                NewStatus = status,
                DetectedAt = DateTime.UtcNow
            });
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating parking space status: {ex.Message}");
            return false;
        }
    }
}