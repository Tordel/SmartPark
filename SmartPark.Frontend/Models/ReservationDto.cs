namespace SmartPark.Frontend.Models;

public class ReservationDto
{
    public Guid Id { get; set; }
    public Guid ParkingSpaceId { get; set; }
    public string SpaceNumber { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public ReservationIntervalDto Interval { get; set; } = new();
    public string Status { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
}