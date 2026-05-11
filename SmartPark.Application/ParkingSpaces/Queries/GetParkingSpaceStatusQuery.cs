using MediatR;
using SmartPark.Application.ParkingSpaces.Queries.DTOs;

namespace SmartPark.Application.ParkingSpaces.Queries;

public record GetParkingSpaceStatusQuery(
    Guid ParkingSpaceId
    ) : IRequest<ParkingSpaceStatusDto>;