
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

public class CompleteReservationCommandTests
{
    [Fact]
    public async Task Handle_ThrowsException_WhenReservationNotFound()
    {
        var reservationId = Guid.NewGuid();
        var mockReservationsRepository = new Mock<IReservationsRepository>();
        var mockSpacesRepository = new Mock<IParkingSpacesRepository>();
        
        mockReservationsRepository.Setup(r => r.GetByIdAsync(reservationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Reservation)null);

        var handler = new CompleteReservationCommandHandler(mockReservationsRepository.Object, mockSpacesRepository.Object);
        var command = new CompleteReservationCommand(reservationId, DateTime.UtcNow, null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CallsRepositoryGetById_WithCorrectReservationId()
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

        var handler = new CompleteReservationCommandHandler(mockReservationsRepository.Object, mockSpacesRepository.Object);
        var command = new CompleteReservationCommand(reservationId, DateTime.UtcNow.AddHours(3), null);

        await Assert.ThrowsAsync<SmartPark.Domain.Exceptions.InvalidReservationStateException>(
            async () => await handler.Handle(command, CancellationToken.None));
        
        mockReservationsRepository.Verify(
            r => r.GetByIdAsync(reservationId, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_CommandContainsCorrectData_WhenCreated()
    {
        var reservationId = Guid.NewGuid();
        var actualEndTime = DateTime.UtcNow.AddHours(3);
        var additionalFees = 5.50m;

        var command = new CompleteReservationCommand(reservationId, actualEndTime, additionalFees);

        Assert.Equal(reservationId, command.ReservationId);
        Assert.Equal(actualEndTime, command.ActualEndTime);
        Assert.Equal(additionalFees, command.AdditionalFees);
    }
}