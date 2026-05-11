using SmartPark.Domain.Exceptions;

namespace SmartPark.Domain.ValueObjects;

public record DateTimeRange
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
    
    public DateTimeRange(DateTime Start, DateTime End)
    {
        if (Start > End) throw new InvalidDateRangeException(Start, End);

        this.Start = Start;
        this.End = End;
    }
    
    public bool OverlapsWith(DateTimeRange other) =>
            Start < other.End && End > other.Start;
}