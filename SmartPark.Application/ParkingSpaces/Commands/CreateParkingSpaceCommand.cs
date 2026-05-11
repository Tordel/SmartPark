using MediatR;
using SmartPark.Domain.Enums;

namespace SmartPark.Application.ParkingSpaces.Commands;

public record CreateParkingSpaceCommand(
    string SpaceNumber,
    decimal HourlyRate,
    SpaceType Type
    ) : IRequest<Guid>;