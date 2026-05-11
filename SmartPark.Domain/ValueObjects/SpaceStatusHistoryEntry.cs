using SmartPark.Domain.Enums;
using SmartPark.Domain.Exceptions;

namespace SmartPark.Domain.ValueObjects;

public record SpaceStatusHistoryEntry
{
    public ParkingSpaceStatus Status { get; init; }
    public DateTimeRange Interval { get; init; }
    public TimeSpan Duration { get; init; }

    public SpaceStatusHistoryEntry(ParkingSpaceStatus status, DateTimeRange interval, TimeSpan duration)
    {
        if (interval.Start >= interval.End)
        {
            throw new InvalidDateRangeException(interval.Start, interval.End);
        }

        Status = status;
        Interval = interval;
        Duration = duration;
    }
    
    private SpaceStatusHistoryEntry() { }
};