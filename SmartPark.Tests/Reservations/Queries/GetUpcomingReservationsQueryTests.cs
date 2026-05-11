using Moq;
using SmartPark.Application.Abstractions;
using SmartPark.Application.Reservations.Queries;
using SmartPark.Application.Reservations.Queries.Handlers;
using SmartPark.Domain.Entities;
using SmartPark.Domain.Enums;
using SmartPark.Domain.ValueObjects;
using Xunit;

namespace SmartPark.Tests.Reservations.Queries;

public class GetUpcomingReservationsQueryTests
{
    [Fact]
    public async Task Handle_ReturnsUpcomingReservations()
    {
        var spaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var futureInterval = new DateTimeRange(DateTime.UtcNow.AddHours(2), DateTime.UtcNow.AddHours(4));
        var reservation = Reservation.Create(spaceId, userId, futureInterval, 10.00m);
        var parkingSpace = ParkingSpace.Create("A1", 5.00m, SpaceType.Standard);
        
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        
        mockReservationsRepository.Setup(r => r.GetActiveReservationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation> { reservation });
        mockSpacesRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parkingSpace);

        var handler = new GetUpcomingReservationsQueryHandler(mockReservationsRepository.Object, mockSpacesRepository.Object);
        var query = new GetUpcomingReservationsQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task Handle_OrdersByStartTime_Ascending()
    {
        var spaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var interval1 = new DateTimeRange(DateTime.UtcNow.AddHours(3), DateTime.UtcNow.AddHours(4));
        var interval2 = new DateTimeRange(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));
        var reservation1 = Reservation.Create(spaceId, userId, interval1, 10.00m);
        var reservation2 = Reservation.Create(spaceId, userId, interval2, 10.00m);
        var parkingSpace = ParkingSpace.Create("A1", 5.00m, SpaceType.Standard);
        
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        
        mockReservationsRepository.Setup(r => r.GetActiveReservationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation> { reservation1, reservation2 });
        mockSpacesRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parkingSpace);

        var handler = new GetUpcomingReservationsQueryHandler(mockReservationsRepository.Object, mockSpacesRepository.Object);
        var query = new GetUpcomingReservationsQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        var resultList = result.ToList();
        Assert.True(resultList[0].Interval.Start < resultList[1].Interval.Start);
    }

    [Fact]
    public async Task Handle_AppliesMaxResults_WhenSpecified()
    {
        var spaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var reservations = Enumerable.Range(1, 10)
            .Select(i => Reservation.Create(spaceId, userId, 
                new DateTimeRange(DateTime.UtcNow.AddHours(i), DateTime.UtcNow.AddHours(i + 1)), 10.00m))
            .ToList();
        var parkingSpace = ParkingSpace.Create("A1", 5.00m, SpaceType.Standard);
        
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        
        mockReservationsRepository.Setup(r => r.GetActiveReservationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(reservations);
        mockSpacesRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parkingSpace);

        var handler = new GetUpcomingReservationsQueryHandler(mockReservationsRepository.Object, mockSpacesRepository.Object);
        var query = new GetUpcomingReservationsQuery(MaxResults: 3);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task Handle_FiltersFromDate_WhenSpecified()
    {
        var spaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var fromDate = DateTime.UtcNow.AddHours(2);
        var interval1 = new DateTimeRange(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));
        var interval2 = new DateTimeRange(fromDate.AddHours(1), fromDate.AddHours(2));
        var reservation1 = Reservation.Create(spaceId, userId, interval1, 10.00m);
        var reservation2 = Reservation.Create(spaceId, userId, interval2, 10.00m);
        var parkingSpace = ParkingSpace.Create("A1", 5.00m, SpaceType.Standard);
        
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        
        mockReservationsRepository.Setup(r => r.GetActiveReservationsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation> { reservation1, reservation2 });
        mockSpacesRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parkingSpace);

        var handler = new GetUpcomingReservationsQueryHandler(mockReservationsRepository.Object, mockSpacesRepository.Object);
        var query = new GetUpcomingReservationsQuery(From: fromDate);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Single(result);
    }
}