using MediatR;
using SmartPark.Domain.Entities;
using SmartPark.Application.Abstractions;

namespace SmartPark.Application.ParkingSpaces.Commands.Handlers;

public class CreateParkingSpaceCommandHandler(IParkingSpacesRepository repository)
    : IRequestHandler<CreateParkingSpaceCommand, Guid>
{
    public async Task<Guid> Handle(CreateParkingSpaceCommand request, CancellationToken cancellationToken)
    {
        var parkingSpace = ParkingSpace.Create(
            request.SpaceNumber,
            request.HourlyRate,
            request.Type
        );

        await repository.AddAsync(parkingSpace, cancellationToken);

        return parkingSpace.Id;
    }
}