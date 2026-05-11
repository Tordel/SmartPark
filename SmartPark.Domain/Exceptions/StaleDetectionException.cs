namespace SmartPark.Domain.Exceptions;

public class StaleDetectionException : Exception
{
    public StaleDetectionException(Guid id, DateTime? detectedAt) : base($"Stale detection for {id} at {detectedAt}.") { } 
}