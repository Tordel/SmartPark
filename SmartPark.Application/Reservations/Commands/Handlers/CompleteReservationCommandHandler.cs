using MediatR;
using SmartPark.Application.Abstractions;

namespace SmartPark.Application.Reservations.Commands.Handlers;

public class CompleteReservationCommandHandler(
    IReservationsRepository reservationsRepository,
    IParkingSpacesRepository parkingSpacesRepository)
    : IRequestHandler<CompleteReservationCommand, Unit>
{
    public async Task<Unit> Handle(CompleteReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await reservationsRepository.GetByIdAsync(request.ReservationId);

        if (reservation == null)
        {
            throw new InvalidOperationException($"Reservation {request.ReservationId} not found.");
        }
        
        reservation.Complete();
        
        var parkingSpace = await parkingSpacesRepository.GetByIdAsync(reservation.ParkingSpaceId);
        parkingSpace.Release();
        
        await reservationsRepository.UpdateAsync(reservation, cancellationToken);
        await parkingSpacesRepository.UpdateAsync(parkingSpace, cancellationToken);
        
        return Unit.Value;
    }
}