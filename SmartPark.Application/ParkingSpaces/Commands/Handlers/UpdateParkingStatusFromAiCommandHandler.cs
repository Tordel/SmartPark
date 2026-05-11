using MediatR;
using SmartPark.Application.Abstractions;

namespace SmartPark.Application.ParkingSpaces.Commands.Handlers;

public class UpdateParkingStatusFromAiCommandHandler(IParkingSpacesRepository repository) : IRequestHandler<UpdateParkingSpaceStatusFromAiCommand, Unit>
{
    public async Task<Unit> Handle(UpdateParkingSpaceStatusFromAiCommand request, CancellationToken cancellationToken)
    {
        var parkingSpace = await repository.GetByIdAsync(request.Id, cancellationToken);

        if (parkingSpace == null)
        {
            throw new InvalidOperationException($"Parking space {request.Id} not found.");
        }
        
        parkingSpace.UpdateOccupancyStatus(request.NewStatus, request.DetectedAt);
        
        await repository.UpdateAsync(parkingSpace, cancellationToken);
        
        return Unit.Value;
    }
}