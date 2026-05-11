using MediatR;
using SmartPark.Application.ParkingSpaces.Queries.DTOs;
using SmartPark.Domain.ValueObjects;

namespace SmartPark.Application.ParkingSpaces.Queries;

public record GetParkingSpaceHistoryQuery(
    Guid ParkingSpaceId,
    DateTimeRange Interval
    ) : IRequest<ParkingSpaceHistoryDto>;