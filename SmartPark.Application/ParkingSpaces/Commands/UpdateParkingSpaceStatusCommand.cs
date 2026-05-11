using MediatR;
using SmartPark.Domain.Enums;

namespace SmartPark.Application.ParkingSpaces.Commands;

public record UpdateParkingSpaceStatusCommand(
    Guid ParkingSpaceId,
    ParkingSpaceStatus NewStatus,
    DateTime? DetectedAt
    )
    : IRequest<Unit>;