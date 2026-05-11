namespace SmartPark.Domain.Exceptions;

public class InvalidHourlyRateException : Exception
{
    public InvalidHourlyRateException() : base("Price cannot be negative.") { }
}