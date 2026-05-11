using MediatR;

namespace SmartPark.Application.ParkingSpaces.Commands;

public record DeleteParkingSpaceCommand(Guid ParkingSpaceId) : IRequest<Unit>;