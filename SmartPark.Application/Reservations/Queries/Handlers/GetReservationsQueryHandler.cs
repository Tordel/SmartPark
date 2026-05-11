using MediatR;
using SmartPark.Application.Abstractions;
using SmartPark.Application.Reservations.Queries.DTOs;
using SmartPark.Domain.Entities;

namespace SmartPark.Application.Reservations.Queries.Handlers;

public class GetReservationsQueryHandler(
    IReservationsRepository repository,
    IParkingSpacesRepository parkingSpacesRepository)
    : IRequestHandler<GetReservationsQuery, IEnumerable<ReservationDto>>
{
    public async Task<IEnumerable<ReservationDto>> Handle(GetReservationsQuery request,
        CancellationToken cancellationToken)
    {
        var reservations = await repository.GetByUserIdAsync(request.UserId, cancellationToken);

        if (request.OnlyActive.HasValue && request.OnlyActive.Value)
        {
            reservations = reservations.Where(r => r.Interval.Start <= DateTime.UtcNow && r.Interval.End >= DateTime.UtcNow);
        }

        var dtos = new List<ReservationDto>();
        foreach (var reservation in reservations)
        {
            var parkingSpace =
                await parkingSpacesRepository.GetByIdAsync(reservation.ParkingSpaceId, cancellationToken);
            dtos.Add(MapToDto(reservation, parkingSpace));
        }

        return dtos;
    }
    
    private ReservationDto MapToDto(Reservation reservation, ParkingSpace parkingSpace)
    {
        return new ReservationDto(
            reservation.Id,
            reservation.ParkingSpaceId,
            parkingSpace.SpaceNumber,
            reservation.CustomerId,
            reservation.Interval,
            reservation.Status,
            reservation.TotalPrice,
            reservation.CreatedAt
        );
    }
}