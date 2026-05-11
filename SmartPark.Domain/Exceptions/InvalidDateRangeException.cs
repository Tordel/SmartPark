namespace SmartPark.Domain.Exceptions;

public class InvalidDateRangeException : Exception
{
    public InvalidDateRangeException(DateTime start, DateTime end) : base("Cannot have a start date after the end date.") { }   
}