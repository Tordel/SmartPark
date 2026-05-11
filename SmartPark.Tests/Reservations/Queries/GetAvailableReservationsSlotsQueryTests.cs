using Moq;
using SmartPark.Application.Abstractions;
using SmartPark.Application.Reservations.Queries;
using SmartPark.Application.Reservations.Queries.Handlers;
using SmartPark.Domain.Entities;
using SmartPark.Domain.Enums;
using SmartPark.Domain.ValueObjects;
using Xunit;

namespace SmartPark.Tests.Reservations.Queries;

public class GetAvailableReservationSlotsQueryTests
{
    [Fact]
    public async Task Handle_ThrowsException_WhenParkingSpaceNotFound()
    {
        var spaceId = Guid.NewGuid();
        var date = DateTime.UtcNow.Date.AddDays(1).AddHours(10);
        
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        
        mockSpacesRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ParkingSpace)null);

        var handler = new GetAvailableReservationSlotsQueryHandler(mockReservationsRepository.Object, mockSpacesRepository.Object);
        var query = new GetAvailableReservationSlotsQuery(spaceId, date);

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CallsRepositories_ToRetrieveData()
    {
        var spaceId = Guid.NewGuid();
        var date = DateTime.UtcNow.Date.AddDays(1).AddHours(10);
        var parkingSpace = ParkingSpace.Create("A1", 5.00m, SpaceType.Standard);
        
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        
        mockSpacesRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parkingSpace);
        mockReservationsRepository.Setup(r => r.GetByParkingSpaceIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation>());

        var handler = new GetAvailableReservationSlotsQueryHandler(mockReservationsRepository.Object, mockSpacesRepository.Object);
        var query = new GetAvailableReservationSlotsQuery(spaceId, date, 60);

 
        await handler.Handle(query, CancellationToken.None);
            
        mockSpacesRepository.Verify(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()), Times.Once);
        mockReservationsRepository.Verify(r => r.GetByParkingSpaceIdAsync(spaceId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsEnumerable_WhenSuccessful()
    {
        var spaceId = Guid.NewGuid();
        var date = DateTime.UtcNow.Date.AddDays(1).AddHours(10);
        var parkingSpace = ParkingSpace.Create("A1", 5.00m, SpaceType.Standard);
        
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        
        mockSpacesRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parkingSpace);
        mockReservationsRepository.Setup(r => r.GetByParkingSpaceIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation>());

        var handler = new GetAvailableReservationSlotsQueryHandler(mockReservationsRepository.Object, mockSpacesRepository.Object);
        var query = new GetAvailableReservationSlotsQuery(spaceId, date, 60);
        
        var result = await handler.Handle(query, CancellationToken.None);
            
        Assert.NotNull(result);
    }
}