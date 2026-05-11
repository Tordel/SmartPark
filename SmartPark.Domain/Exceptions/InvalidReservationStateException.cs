using SmartPark.Domain.Enums;

namespace SmartPark.Domain.Exceptions;

public class InvalidReservationStateException : Exception
{
    public InvalidReservationStateException(Guid reservationId, ReservationStatus status, ReservationStatus attempted) :
        base($"Reservation {reservationId} cannot be changed to {attempted} from {status}.") { }
}