using Moq;
using SmartPark.Application.Abstractions;
using SmartPark.Application.ParkingSpaces.Queries;
using SmartPark.Application.ParkingSpaces.Queries.DTOs;
using SmartPark.Application.ParkingSpaces.Queries.Handlers;
using SmartPark.Domain.Entities;
using SmartPark.Domain.Enums;
using SmartPark.Domain.ValueObjects;
using Xunit;

namespace SmartPark.Tests.ParkingSpaces.Queries;

public class GetParkingSpaceByIdQueryTests
{
    [Fact]
    public async Task Handle_ReturnsParkingSpaceDto_WhenSpaceExists()
    {
        var spaceId = Guid.NewGuid();
        var parkingSpace = ParkingSpace.Create("A1", 5.00m, SpaceType.Standard);
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        
        mockSpacesRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parkingSpace);
        mockReservationsRepository.Setup(r => r.GetByParkingSpaceIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation>());

        var handler = new GetParkingSpaceByIdQueryHandler(mockSpacesRepository.Object, mockReservationsRepository.Object);
        var query = new GetParkingSpaceByIdQuery(spaceId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("A1", result.SpaceNumber);
        Assert.Equal(SpaceType.Standard, result.Type);
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenSpaceNotFound()
    {
        var spaceId = Guid.NewGuid();
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        
        mockSpacesRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ParkingSpace)null);

        var handler = new GetParkingSpaceByIdQueryHandler(mockSpacesRepository.Object, mockReservationsRepository.Object);
        var query = new GetParkingSpaceByIdQuery(spaceId);

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_IncludesReservationCounts_InResult()
    {
        var spaceId = Guid.NewGuid();
        var parkingSpace = ParkingSpace.Create("B2", 6.00m, SpaceType.EV);
        var userId = Guid.NewGuid();
        var interval = new DateTimeRange(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));
        var reservation = Reservation.Create(spaceId, userId, interval, 10.00m);

        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        
        mockSpacesRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parkingSpace);
        mockReservationsRepository.Setup(r => r.GetByParkingSpaceIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation> { reservation });

        var handler = new GetParkingSpaceByIdQueryHandler(mockSpacesRepository.Object, mockReservationsRepository.Object);
        var query = new GetParkingSpaceByIdQuery(spaceId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(1, result.TotalReservations);
    }
}