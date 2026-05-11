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

public class ExtendReservationCommandTests
{
    [Fact]
    public async Task Handle_ThrowsException_WhenReservationNotFound()
    {
        var reservationId = Guid.NewGuid();
        var newInterval = new DateTimeRange(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(5));
        
        var mockRepository = new Mock<IReservationsRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(reservationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Reservation)null);

        var handler = new ExtendReservationCommandHandler(mockRepository.Object);
        var command = new ExtendReservationCommand(reservationId, newInterval, "Customer requested extension");

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CallsRepositoryGetById_WithCorrectReservationId()
    {
        var reservationId = Guid.NewGuid();
        var spaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var originalInterval = new DateTimeRange(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(3));
        var reservation = Reservation.Create(spaceId, userId, originalInterval, 10.00m);
        var newInterval = new DateTimeRange(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(5));
        
        var mockRepository = new Mock<IReservationsRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(reservationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reservation);

        var handler = new ExtendReservationCommandHandler(mockRepository.Object);
        var command = new ExtendReservationCommand(reservationId, newInterval, "Customer requested extension");

        await Assert.ThrowsAsync<SmartPark.Domain.Exceptions.InvalidReservationStateException>(
            async () => await handler.Handle(command, CancellationToken.None));
        
        mockRepository.Verify(
            r => r.GetByIdAsync(reservationId, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_CommandContainsCorrectData_WhenCreated()
    {
        var reservationId = Guid.NewGuid();
        var newInterval = new DateTimeRange(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(6));
        var extensionReason = "Customer requested extension";

        var command = new ExtendReservationCommand(reservationId, newInterval, extensionReason);

        Assert.Equal(reservationId, command.ReservationId);
        Assert.Equal(newInterval, command.NewInterval);
        Assert.Equal(extensionReason, command.ExtensionReason);
    }
}