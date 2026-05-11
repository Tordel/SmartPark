namespace SmartPark.Domain.Exceptions;

public class ReservationOverlapException : Exception
{
    public ReservationOverlapException(Guid id) : base($"Reservation {id} overlaps with another reservation.") { }
}