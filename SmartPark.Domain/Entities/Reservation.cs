using SmartPark.Domain.Enums;
using SmartPark.Domain.Exceptions;
using SmartPark.Domain.ValueObjects;

namespace SmartPark.Domain.Entities;

public class Reservation
{
    public Guid Id { get; private set; }
    public Guid ParkingSpaceId { get; private set; }
    public Guid CustomerId { get; private set; }
    public DateTimeRange Interval { get; private set; }
    public ReservationStatus Status { get; private set; }
    public decimal TotalPrice { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    private Reservation() { }

    public static Reservation Create(Guid parkingSpaceId, Guid customerId, DateTimeRange interval, decimal totalPrice)
    {
        if (totalPrice < 0)
        {
            throw new NegativePriceException();
        }

        return new Reservation
        {
            Id = Guid.NewGuid(),
            ParkingSpaceId = parkingSpaceId,
            CustomerId = customerId,
            Interval = interval,
            Status = ReservationStatus.Pending,
            TotalPrice = totalPrice,
            CreatedAt = DateTime.UtcNow
        };
    }
    
    public void Confirm()
    {
        if (Status != ReservationStatus.Pending)
        {
            throw new InvalidReservationStateException(Id, Status, ReservationStatus.Confirmed);
        }
        
        Status = ReservationStatus.Confirmed;
    }

    public void Cancel()
    {
        if (Status != ReservationStatus.Pending)
        {
            throw new InvalidReservationStateException(Id, Status, ReservationStatus.Cancelled);
        }
        
        Status = ReservationStatus.Cancelled;   
    }
    
    public void Complete()
    {
        if (Status != ReservationStatus.Confirmed)
        {
            throw new InvalidReservationStateException(Id, Status, ReservationStatus.Completed);
        }
        
        Status = ReservationStatus.Completed;
    }

    public void Extend(DateTimeRange newInterval)
    {
        if (Status != ReservationStatus.Confirmed)
        {
            throw new InvalidReservationStateException(Id, Status, ReservationStatus.Confirmed);
        }
        
        Interval = newInterval;
    }

    public void Expire()
    {
        
    }
    
    public bool OverlapsWith(Reservation other) => 
        Interval.OverlapsWith(other.Interval);
    
    public bool OverlapsWith(DateTimeRange range) =>
        Interval.OverlapsWith(range);
    
    public bool IsActive() => 
        Status == ReservationStatus.Confirmed ||
        Status == ReservationStatus.Pending;
}