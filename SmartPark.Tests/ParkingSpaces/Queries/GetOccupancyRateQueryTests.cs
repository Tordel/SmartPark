using Moq;
using SmartPark.Application.Abstractions;
using SmartPark.Application.ParkingSpaces.Queries;
using SmartPark.Application.ParkingSpaces.Queries.Handlers;
using SmartPark.Domain.Entities;
using SmartPark.Domain.Enums;
using SmartPark.Domain.ValueObjects;
using Xunit;

namespace SmartPark.Tests.ParkingSpaces.Queries;

public class GetOccupancyRateQueryTests
{
    [Fact]
    public async Task Handle_ReturnsOccupancyRateDto_WhenDataExists()
    {
        var spaceId = Guid.NewGuid();
        var space = ParkingSpace.Create("A1", 5.00m, SpaceType.Standard);
        var spaces = new List<ParkingSpace> { space };
        var reservations = new List<Reservation>();
        
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        
        mockSpacesRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(spaces);
        mockReservationsRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(reservations);

        var handler = new GetOccupancyRateQueryHandler(mockSpacesRepository.Object, mockReservationsRepository.Object);
        var query = new GetOccupancyRateQuery(
            new DateTimeRange(DateTime.UtcNow, DateTime.UtcNow.AddHours(24)));

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.OccupancyPercentage >= 0 && result.OccupancyPercentage <= 100);
    }

    [Fact]
    public async Task Handle_WithNoReservations_ReturnsZeroOccupancy()
    {
        var space = ParkingSpace.Create("A1", 5.00m, SpaceType.Standard);
        var spaces = new List<ParkingSpace> { space };
        var reservations = new List<Reservation>();
        
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        
        mockSpacesRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(spaces);
        mockReservationsRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(reservations);

        var handler = new GetOccupancyRateQueryHandler(mockSpacesRepository.Object, mockReservationsRepository.Object);
        var query = new GetOccupancyRateQuery(
            new DateTimeRange(DateTime.UtcNow, DateTime.UtcNow.AddHours(24)));

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(0, result.OccupancyPercentage);
    }

    [Fact]
    public async Task Handle_IncludesHourlyBreakdown_InResult()
    {
        var space = ParkingSpace.Create("A1", 5.00m, SpaceType.Standard);
        var spaces = new List<ParkingSpace> { space };
        var reservations = new List<Reservation>();
        
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        
        mockSpacesRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(spaces);
        mockReservationsRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(reservations);

        var handler = new GetOccupancyRateQueryHandler(mockSpacesRepository.Object, mockReservationsRepository.Object);
        var query = new GetOccupancyRateQuery(
            new DateTimeRange(DateTime.UtcNow, DateTime.UtcNow.AddHours(4)));

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotEmpty(result.HourlyBreakdown);
    }
}