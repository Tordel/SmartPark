using MediatR;
using SmartPark.Application.Abstractions;

namespace SmartPark.Application.Reservations.Commands.Handlers;

public class ExtendReservationCommandHandler : IRequestHandler<ExtendReservationCommand, Unit>
{
    private readonly IReservationsRepository _reservationsRepository;

    public ExtendReservationCommandHandler(IReservationsRepository reservationsRepository)
    {
        _reservationsRepository = reservationsRepository;   
    }

    public async Task<Unit> Handle(ExtendReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _reservationsRepository.GetByIdAsync(request.ReservationId);

        if (reservation == null)
        {
            throw new InvalidOperationException($"Reservation {request.ReservationId} not found.");
        }
        
        reservation.Extend(request.NewInterval);
        
        await _reservationsRepository.UpdateAsync(reservation, cancellationToken);
        
        return Unit.Value;
    }
}