using MediatR;
using SmartPark.Application.Abstractions;

namespace SmartPark.Application.ParkingSpaces.Commands.Handlers;

public class DeleteParkingSpaceCommandHandler(IParkingSpacesRepository repository)
    : IRequestHandler<DeleteParkingSpaceCommand, Unit>
{
    public async Task<Unit> Handle(DeleteParkingSpaceCommand request, CancellationToken cancellationToken)
    {
        var parkingSpace = await repository.GetByIdAsync(request.ParkingSpaceId, cancellationToken);
        
        if (parkingSpace == null)
            throw new InvalidOperationException($"Parking space {request.ParkingSpaceId} not found.");
       
        if (parkingSpace.Reservations.Any(r => r.IsActive()))
            throw new InvalidOperationException($"Parking space {request.ParkingSpaceId} is in use.");
        
        await repository.DeleteAsync(request.ParkingSpaceId, cancellationToken);
        
        return Unit.Value;
    }
}