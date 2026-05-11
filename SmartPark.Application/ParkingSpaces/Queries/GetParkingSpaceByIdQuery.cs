using MediatR;
using SmartPark.Application.ParkingSpaces.Queries.DTOs;

namespace SmartPark.Application.ParkingSpaces.Queries;

public record GetParkingSpaceByIdQuery(
    Guid ParkingSpaceId
    ) : IRequest<ParkingSpaceDetailsDto>;