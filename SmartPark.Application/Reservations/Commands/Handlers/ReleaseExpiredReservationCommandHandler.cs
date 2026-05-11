using MediatR;
using SmartPark.Application.Abstractions;

namespace SmartPark.Application.Reservations.Commands.Handlers;

public class ReleaseExpiredReservationCommandHandler : IRequestHandler<ReleaseExpiredReservationCommand, Unit>
{
    private readonly IReservationsRepository _reservationsRepository;
    private readonly IParkingSpacesRepository _parkingSpacesRepository;
    
    public ReleaseExpiredReservationCommandHandler(IReservationsRepository reservationsRepository, IParkingSpacesRepository parkingSpacesRepository)
    {
        _reservationsRepository = reservationsRepository;
        _parkingSpacesRepository = parkingSpacesRepository;
    }

    public async Task<Unit> Handle(ReleaseExpiredReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _reservationsRepository.GetByIdAsync(request.ReservationId);

        if (reservation == null)
        {
            throw new InvalidOperationException($"Reservation {request.ReservationId} not found.");
        }
        
        reservation.Expire();
        
        var parkingSpace = await _parkingSpacesRepository.GetByIdAsync(reservation.ParkingSpaceId);
        parkingSpace.Release();
        
        await _reservationsRepository.UpdateAsync(reservation, cancellationToken);
        await _parkingSpacesRepository.UpdateAsync(parkingSpace, cancellationToken);
        
        return Unit.Value;
    }
}