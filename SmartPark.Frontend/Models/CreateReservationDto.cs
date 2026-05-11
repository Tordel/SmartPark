namespace SmartPark.Frontend.Models;

public class CreateReservationDto
{
    public Guid ParkingSpaceId { get; set; }
    public Guid UserId { get; set; }
    public DateTimeRangeDto Interval { get; set; } = new();
    public decimal TotalPrice { get; set; }
}