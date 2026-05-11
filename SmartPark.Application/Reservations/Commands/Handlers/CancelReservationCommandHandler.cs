using MediatR;
using SmartPark.Domain.Entities;
using SmartPark.Application.Abstractions;

namespace SmartPark.Application.Reservations.Commands.Handlers;

public class CancelReservationCommandHandler(
    IReservationsRepository reservationsRepository,
    IParkingSpacesRepository parkingSpacesRepository)
    : IRequestHandler<CancelReservationCommand, Unit>
{
    public async Task<Unit> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await reservationsRepository.GetByIdAsync(request.ReservationId);
        
        if (reservation == null) {
            throw new InvalidOperationException($"Reservation {request.ReservationId} not found.");
            
        }
        
        reservation.Cancel();

        var parkingSpace = await parkingSpacesRepository.GetByIdAsync(reservation.ParkingSpaceId);
        parkingSpace.Release();
        
        await reservationsRepository.UpdateAsync(reservation, cancellationToken);
        await parkingSpacesRepository.UpdateAsync(parkingSpace, cancellationToken);
        
        return Unit.Value;
    }
}