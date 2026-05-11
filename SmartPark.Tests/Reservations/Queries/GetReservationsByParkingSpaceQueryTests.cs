using Moq;
using SmartPark.Application.Abstractions;
using SmartPark.Application.Reservations.Queries;
using SmartPark.Application.Reservations.Queries.Handlers;
using SmartPark.Domain.Entities;
using SmartPark.Domain.Enums;
using SmartPark.Domain.ValueObjects;
using Xunit;

namespace SmartPark.Tests.Reservations.Queries;

public class GetReservationsByParkingSpaceQueryTests
{
    [Fact]
    public async Task Handle_ReturnsReservationsForParkingSpace()
    {
        var spaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var interval = new DateTimeRange(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(3));
        var reservation = Reservation.Create(spaceId, userId, interval, 10.00m);
        var parkingSpace = ParkingSpace.Create("A1", 5.00m, SpaceType.Standard);
        
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        
        mockReservationsRepository.Setup(r => r.GetByParkingSpaceIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation> { reservation });
        mockSpacesRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parkingSpace);

        var handler = new GetReservationsByParkingSpaceQueryHandler(mockReservationsRepository.Object, mockSpacesRepository.Object);
        var query = new GetReservationsByParkingSpaceQuery(spaceId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenParkingSpaceNotFound()
    {
        var spaceId = Guid.NewGuid();
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        
        mockSpacesRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ParkingSpace)null);

        var handler = new GetReservationsByParkingSpaceQueryHandler(mockReservationsRepository.Object, mockSpacesRepository.Object);
        var query = new GetReservationsByParkingSpaceQuery(spaceId);

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_FiltersFromDate_WhenSpecified()
    {
        var spaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var fromDate = DateTime.UtcNow;
        var pastInterval = new DateTimeRange(DateTime.UtcNow.AddHours(-2), DateTime.UtcNow.AddHours(-1));
        var futureInterval = new DateTimeRange(fromDate.AddHours(1), fromDate.AddHours(2));
        var pastReservation = Reservation.Create(spaceId, userId, pastInterval, 10.00m);
        var futureReservation = Reservation.Create(spaceId, userId, futureInterval, 10.00m);
        var parkingSpace = ParkingSpace.Create("A1", 5.00m, SpaceType.Standard);
        
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        
        mockReservationsRepository.Setup(r => r.GetByParkingSpaceIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation> { pastReservation, futureReservation });
        mockSpacesRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parkingSpace);

        var handler = new GetReservationsByParkingSpaceQueryHandler(mockReservationsRepository.Object, mockSpacesRepository.Object);
        var query = new GetReservationsByParkingSpaceQuery(spaceId, From: fromDate);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task Handle_FiltersToDate_WhenSpecified()
    {
        var spaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var toDate = DateTime.UtcNow.AddHours(2);
        var reservationBeforeTo = new DateTimeRange(DateTime.UtcNow.AddHours(1), toDate);
        var reservationAfterTo = new DateTimeRange(toDate.AddHours(1), toDate.AddHours(2));
        var earlierReservation = Reservation.Create(spaceId, userId, reservationBeforeTo, 10.00m);
        var laterReservation = Reservation.Create(spaceId, userId, reservationAfterTo, 10.00m);
        var parkingSpace = ParkingSpace.Create("A1", 5.00m, SpaceType.Standard);
        
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        
        mockReservationsRepository.Setup(r => r.GetByParkingSpaceIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation> { earlierReservation, laterReservation });
        mockSpacesRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parkingSpace);

        var handler = new GetReservationsByParkingSpaceQueryHandler(mockReservationsRepository.Object, mockSpacesRepository.Object);
        var query = new GetReservationsByParkingSpaceQuery(spaceId, To: toDate);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Single(result);
    }
}