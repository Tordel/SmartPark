using System.Net.Http.Json;
using SmartPark.Frontend.Models;

namespace SmartPark.Frontend.Services;

public class ReservationService
{
    private readonly HttpClient _httpClient;
    
    public ReservationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ReservationDto>?> GetUserReservationsAsync(string userId, bool? onlyActive = null)
    {
        try
        {
            var url = $"api/reservations/user/{userId}";
            if (onlyActive.HasValue)
                url += $"?onlyActive={onlyActive}";
            return await _httpClient.GetFromJsonAsync<List<ReservationDto>>(url);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching user reservations: {ex.Message}");
            return null;
        }
    }

    public async Task<List<ReservationDto>?> GetReservationsByParkingSpaceAsync(Guid parkingSpaceId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<ReservationDto>>(
                $"api/reservations/by-parking-space/{parkingSpaceId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching reservations by parking space: {ex.Message}");
            return null;
        }   
    }
    
    public async Task<bool> CreateReservationAsync(CreateReservationDto request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/reservations", request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating reservation: {ex.Message}");
            return false;
        }
    }
    
    public async Task<bool> CancelReservationAsync(Guid reservationId)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync(
                $"api/reservations/{reservationId}/cancel",
                new { 
                    ReservationId = reservationId,
                    CancelReason = "Cancelled by user",
                    CancelledAt = DateTime.UtcNow
                });
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cancelling reservation: {ex.Message}");
            return false;
        }
    }
}