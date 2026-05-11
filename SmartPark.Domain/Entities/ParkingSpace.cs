using System.Runtime.InteropServices;
using SmartPark.Domain.Enums;
using SmartPark.Domain.Exceptions;

namespace SmartPark.Domain.Entities;

public class ParkingSpace
{
    public Guid Id { get; private set; }
    public string SpaceNumber { get; private set; }
    public ParkingSpaceStatus Status { get; private set; }
    public SpaceType Type { get; private set; }
    public DateTime? LastDetectedAt { get; private set; }
    public decimal HourlyRate { get; private set; }

    private ParkingSpace() { }

    private readonly List<Reservation> _reservations = new();
    public IReadOnlyCollection<Reservation> Reservations => _reservations.AsReadOnly();

    public static ParkingSpace Create(string spaceNumber, decimal hourlyRate, SpaceType type)
    {
        if (string.IsNullOrEmpty(spaceNumber))
        {
            throw new ArgumentNullException();
        }

        if (hourlyRate < 0)
        {
            throw new InvalidHourlyRateException();
        }

        return new ParkingSpace
        {
            Id = Guid.NewGuid(),
            SpaceNumber = spaceNumber,
            Type = type,
            Status = ParkingSpaceStatus.Free,
            HourlyRate = hourlyRate
        };
    }


    public void Reserve(Reservation reservation)
    {
        if (Status == ParkingSpaceStatus.Occupied)
        {
            throw new SpaceAlreadyOccupiedException(Id);
        }

        if (_reservations.Any(r => r.OverlapsWith(reservation)))
        {
            throw new ReservationOverlapException(Id);
        }
    
        Status = ParkingSpaceStatus.Reserved;
        _reservations.Add(reservation);
    }

    public void UpdateOccupancyStatus(ParkingSpaceStatus detectedStatus, DateTime? detectedAt)
    {
        if (detectedAt < LastDetectedAt)
        {
            throw new StaleDetectionException(Id, detectedAt);
        }
        
        Status = detectedStatus;
        LastDetectedAt = detectedAt;
    }

    public void Release()
    {
        Status = ParkingSpaceStatus.Free;
    }
}