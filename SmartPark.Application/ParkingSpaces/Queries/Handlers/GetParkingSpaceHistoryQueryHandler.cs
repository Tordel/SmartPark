using MediatR;
using SmartPark.Application.Abstractions;
using SmartPark.Application.ParkingSpaces.Queries.DTOs;
using SmartPark.Domain.Enums;
using SmartPark.Domain.ValueObjects;

namespace SmartPark.Application.ParkingSpaces.Queries.Handlers;

public class GetParkingSpaceHistoryQueryHandler(
    IParkingSpacesRepository repository,
    IReservationsRepository reservationsRepository)
    : IRequestHandler<GetParkingSpaceHistoryQuery, ParkingSpaceHistoryDto>
{
    public async Task<ParkingSpaceHistoryDto> Handle(GetParkingSpaceHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var space = await repository.GetByIdAsync(request.ParkingSpaceId, cancellationToken);

        if (space == null)
        {
            throw new InvalidOperationException($"Parking space {request.ParkingSpaceId} not found.");
        }

        var reservations =
            await reservationsRepository.GetByParkingSpaceIdAsync(request.ParkingSpaceId, cancellationToken);

        var statusHistory = new List<SpaceStatusHistoryEntry>();
        var occupiedTime = TimeSpan.Zero;
        var freeTime = TimeSpan.Zero;
        
        var periodReservations = reservations
            .Where(r => r.Interval.Start < request.Interval.End && r.Interval.End > request.Interval.Start)
            .OrderBy(r => r.Interval.Start)
            .ToList();

        var currentTime = request.Interval.Start;

        foreach (var reservation in periodReservations)
        {
            if (currentTime < reservation.Interval.Start)
            {
                var freeDuration = reservation.Interval.Start - currentTime;
                freeTime += freeDuration;
                statusHistory.Add(new SpaceStatusHistoryEntry(
                    ParkingSpaceStatus.Free,
                    new DateTimeRange(currentTime, reservation.Interval.Start),
                    freeDuration
                    ));
            }

            var occupiedDuration = reservation.Interval.End - reservation.Interval.Start;
            occupiedTime += occupiedDuration;
            statusHistory.Add(new SpaceStatusHistoryEntry(
                ParkingSpaceStatus.Occupied,
                new DateTimeRange(reservation.Interval.Start, reservation.Interval.End),
                occupiedDuration
                ));
            
            currentTime = reservation.Interval.End;
        }

        if (currentTime < request.Interval.End)
        {
            var remainingFree = request.Interval.End - currentTime;
            freeTime += remainingFree;
            statusHistory.Add(new SpaceStatusHistoryEntry(
                ParkingSpaceStatus.Free,
                new DateTimeRange(currentTime, request.Interval.End),
                remainingFree
                ));
        }

        var totalTime = occupiedTime + freeTime;
        var occupancyPercentage = totalTime.TotalHours > 0
            ? (decimal)(occupiedTime.TotalHours / totalTime.TotalHours) * 100m
            : 0;
        
        return new ParkingSpaceHistoryDto(
            space.Id,
            space.SpaceNumber,
            request.Interval,
            statusHistory,
            occupiedTime,
            freeTime,
            occupancyPercentage
            );
    }
}