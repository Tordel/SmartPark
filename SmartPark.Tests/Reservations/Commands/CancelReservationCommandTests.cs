using MediatR;
using Moq;
using SmartPark.Application.Abstractions;
using SmartPark.Application.Reservations.Commands;
using SmartPark.Application.Reservations.Commands.Handlers;
using SmartPark.Domain.Entities;
using SmartPark.Domain.Enums;
using SmartPark.Domain.ValueObjects;
using Xunit;

namespace SmartPark.Tests.Reservations.Commands;

public class CancelReservationCommandTests
{
    [Fact]
    public async Task Handle_CancelsReservation_WhenReservationExists()
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

        var handler = new CancelReservationCommandHandler(mockReservationsRepository.Object, mockSpacesRepository.Object);
        var command = new CancelReservationCommand(reservationId, "User requested cancellation", DateTime.UtcNow);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(Unit.Value, result);
        mockReservationsRepository.Verify(r => r.UpdateAsync(reservation, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenReservationNotFound()
    {
        var reservationId = Guid.NewGuid();
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        
        mockReservationsRepository.Setup(r => r.GetByIdAsync(reservationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Reservation)null);

        var handler = new CancelReservationCommandHandler(mockReservationsRepository.Object, mockSpacesRepository.Object);
        var command = new CancelReservationCommand(reservationId, "User requested cancellation", DateTime.UtcNow);

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ReleasesParkingSpace_OnSuccess()
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

        var handler = new CancelReservationCommandHandler(mockReservationsRepository.Object, mockSpacesRepository.Object);
        var command = new CancelReservationCommand(reservationId, "User requested cancellation", DateTime.UtcNow);

        await handler.Handle(command, CancellationToken.None);

        mockSpacesRepository.Verify(r => r.UpdateAsync(parkingSpace, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("User requested cancellation")]
    [InlineData("Emergency")]
    [InlineData("System maintenance")]
    public async Task Handle_WithVariousCancelReasons_CancelsSuccessfully(string cancelReason)
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

        var handler = new CancelReservationCommandHandler(mockReservationsRepository.Object, mockSpacesRepository.Object);
        var command = new CancelReservationCommand(reservationId, cancelReason, DateTime.UtcNow);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(Unit.Value, result);
    }
}