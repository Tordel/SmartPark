using MediatR;
using SmartPark.Domain.Enums;

namespace SmartPark.Application.ParkingSpaces.Commands;

public record UpdateParkingSpaceStatusFromAiCommand(
    Guid Id,
    ParkingSpaceStatus NewStatus,
    DateTime? DetectedAt,
    decimal? Confidence = null,
    string? Source = null
    ) : IRequest<Unit>;