using Moq;
using SmartPark.Application.Abstractions;
using SmartPark.Application.Reservations.Queries;
using SmartPark.Application.Reservations.Queries.DTOs;
using SmartPark.Application.Reservations.Queries.Handlers;
using SmartPark.Domain.Entities;
using SmartPark.Domain.Enums;
using SmartPark.Domain.ValueObjects;
using Xunit;

namespace SmartPark.Tests.Reservations.Queries;

public class GetReservationByIdQueryTests
{
    [Fact]
    public async Task Handle_ReturnsReservationDto_WhenReservationExists()
    {
        var reservationId = Guid.NewGuid();
        var spaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var interval = new DateTimeRange(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(3));
        var reservation = Reservation.Create(spaceId, userId, interval, 10.00m);
        var parkingSpace = ParkingSpace.Create("A1", 5.00m, SpaceType.Standard);
        
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        
        mockReservationsRepository.Setup(r => r.GetByIdAsync(reservationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reservation);
        mockSpacesRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parkingSpace);

        var handler = new GetReservationByIdQueryHandler(mockReservationsRepository.Object, mockSpacesRepository.Object);
        var query = new GetReservationByIdQuery(reservationId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(spaceId, result.ParkingSpaceId);
        Assert.Equal("A1", result.SpaceNumber);
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenReservationNotFound()
    {
        var reservationId = Guid.NewGuid();
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        
        mockReservationsRepository.Setup(r => r.GetByIdAsync(reservationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Reservation)null);

        var handler = new GetReservationByIdQueryHandler(mockReservationsRepository.Object, mockSpacesRepository.Object);
        var query = new GetReservationByIdQuery(reservationId);

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_IncludesAllReservationDetails_InResult()
    {
        var reservationId = Guid.NewGuid();
        var spaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var interval = new DateTimeRange(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(3));
        var totalPrice = 25.50m;
        var reservation = Reservation.Create(spaceId, userId, interval, totalPrice);
        var parkingSpace = ParkingSpace.Create("B2", 6.00m, SpaceType.EV);
        
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        
        mockReservationsRepository.Setup(r => r.GetByIdAsync(reservationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reservation);
        mockSpacesRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parkingSpace);

        var handler = new GetReservationByIdQueryHandler(mockReservationsRepository.Object, mockSpacesRepository.Object);
        var query = new GetReservationByIdQuery(reservationId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(totalPrice, result.TotalPrice);
    }
}