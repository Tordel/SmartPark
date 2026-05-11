using MediatR;
using SmartPark.Application.Abstractions;

namespace SmartPark.Application.ParkingSpaces.Commands.Handlers;

public class UpdateParkingSpaceStatusCommandHandler(IParkingSpacesRepository repository)
    : IRequestHandler<UpdateParkingSpaceStatusCommand, Unit>
{
    public async Task<Unit> Handle(UpdateParkingSpaceStatusCommand request, CancellationToken cancellationToken)
    {
        var parkingSpace = await repository.GetByIdAsync(request.ParkingSpaceId, cancellationToken);

        if (parkingSpace == null)
        {
            throw new InvalidOperationException($"Parking space {request.ParkingSpaceId} not found.");
        }
        
        parkingSpace.UpdateOccupancyStatus(request.NewStatus, request.DetectedAt);
        
        await repository.UpdateAsync(parkingSpace, cancellationToken);
        
        return Unit.Value;
    }
}