using Moq;
using SmartPark.Application.Abstractions;
using SmartPark.Application.Reservations.Commands;
using SmartPark.Application.Reservations.Commands.Handlers;
using SmartPark.Domain.Entities;
using SmartPark.Domain.Enums;
using SmartPark.Domain.ValueObjects;
using Xunit;

namespace SmartPark.Tests.Reservations.Commands;

public class CreateReservationCommandTests
{
    [Fact]
    public async Task Handle_CreatesNewReservation_WithValidInput()
    {
        var spaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var interval = new DateTimeRange(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(3));
        var parkingSpace = ParkingSpace.Create("A1", 5.00m, SpaceType.Standard);
        
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        
        mockSpacesRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parkingSpace);

        var handler = new CreateReservationCommandHandler(mockSpacesRepository.Object, mockReservationsRepository.Object);
        var command = new CreateReservationCommand(spaceId, userId, interval, 10.00m);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result);
        mockReservationsRepository.Verify(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenParkingSpaceNotFound()
    {
        var spaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var interval = new DateTimeRange(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(3));
        
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        
        mockSpacesRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ParkingSpace)null);

        var handler = new CreateReservationCommandHandler(mockSpacesRepository.Object, mockReservationsRepository.Object);
        var command = new CreateReservationCommand(spaceId, userId, interval, 10.00m);

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ReserveParkingSpace_OnSuccess()
    {
        var spaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var interval = new DateTimeRange(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(3));
        var parkingSpace = ParkingSpace.Create("A1", 5.00m, SpaceType.Standard);
        
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        
        mockSpacesRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parkingSpace);

        var handler = new CreateReservationCommandHandler(mockSpacesRepository.Object, mockReservationsRepository.Object);
        var command = new CreateReservationCommand(spaceId, userId, interval, 10.00m);

        await handler.Handle(command, CancellationToken.None);

        mockSpacesRepository.Verify(r => r.UpdateAsync(parkingSpace, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(10.00)]
    [InlineData(25.50)]
    [InlineData(100.00)]
    public async Task Handle_WithVariousPrices_CreatesSuccessfully(decimal totalPrice)
    {
        var spaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var interval = new DateTimeRange(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(3));
        var parkingSpace = ParkingSpace.Create("A1", 5.00m, SpaceType.Standard);
        
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        
        mockSpacesRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parkingSpace);

        var handler = new CreateReservationCommandHandler(mockSpacesRepository.Object, mockReservationsRepository.Object);
        var command = new CreateReservationCommand(spaceId, userId, interval, totalPrice);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result);
    }
}