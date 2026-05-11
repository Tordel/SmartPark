using MediatR;
using SmartPark.Application.Abstractions;
using SmartPark.Domain.Entities;

namespace SmartPark.Application.Reservations.Commands.Handlers;

public class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, Guid>
{
    private readonly IParkingSpacesRepository _parkingSpacesRepository;
    private readonly IReservationsRepository _reservationsRepository;
    
    public CreateReservationCommandHandler(IParkingSpacesRepository parkingSpacesRepository, IReservationsRepository reservationsRepository)
    {
        _parkingSpacesRepository = parkingSpacesRepository;
        _reservationsRepository = reservationsRepository;
    }

    public async Task<Guid> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        var parkingSpace = await _parkingSpacesRepository.GetByIdAsync(request.ParkingSpaceId, cancellationToken);

        if (parkingSpace == null)
        {
            throw new InvalidOperationException($"Parking space {request.ParkingSpaceId} not found.");
        }

        var reservation = Reservation.Create(
            request.ParkingSpaceId,
            request.UserId, 
            request.Interval,
            request.TotalPrice
            );
            
        parkingSpace.Reserve(reservation);

        await _reservationsRepository.AddAsync(reservation, cancellationToken);
        await _parkingSpacesRepository.UpdateAsync(parkingSpace, cancellationToken);
        
        return reservation.Id;
    }
}