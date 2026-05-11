using MediatR;
using Moq;
using SmartPark.Application.Abstractions;
using SmartPark.Application.ParkingSpaces.Commands;
using SmartPark.Application.ParkingSpaces.Commands.Handlers;
using SmartPark.Domain.Entities;
using SmartPark.Domain.Enums;
using SmartPark.Domain.ValueObjects;
using Xunit;

namespace SmartPark.Tests.ParkingSpaces.Commands;

public class DeleteParkingSpaceCommandTests
{
    [Fact]
    public async Task Handle_DeletesParkingSpace_WhenNoActiveReservations()
    {
        var spaceId = Guid.NewGuid();
        var parkingSpace = ParkingSpace.Create("A1", 5.00m, SpaceType.Standard);
        var mockRepository = new Mock<IParkingSpacesRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parkingSpace);
        
        var handler = new DeleteParkingSpaceCommandHandler(mockRepository.Object);
        var command = new DeleteParkingSpaceCommand(spaceId);

        var result = await handler.Handle(command, CancellationToken.None);
        
        Assert.Equal(Unit.Value, result);
        mockRepository.Verify(r => r.DeleteAsync(spaceId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenParkingSpaceNotFound()
    {
        var spaceId = Guid.NewGuid();
        var mockRepository = new Mock<IParkingSpacesRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ParkingSpace?)null);
        
        var handler = new DeleteParkingSpaceCommandHandler(mockRepository.Object);
        var command = new DeleteParkingSpaceCommand(spaceId);
        
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenActiveReservationsExist()
    {
        var spaceId = Guid.NewGuid();
        var parkingSpace = ParkingSpace.Create("B2", 6.00m, SpaceType.Standard);
        var userId = Guid.NewGuid();
        var interval = new DateTimeRange(DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        var reservation = Reservation.Create(spaceId, userId, interval, 10.00m);
        parkingSpace.Reserve(reservation);
        
        var mockRepository = new Mock<IParkingSpacesRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parkingSpace);
        
        var handler = new DeleteParkingSpaceCommandHandler(mockRepository.Object);
        var command = new DeleteParkingSpaceCommand(spaceId);
        
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
        mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CallsRepositoryDelete_WithCorrectId()
    {
        var spaceId = Guid.NewGuid();
        var parkingSpace = ParkingSpace.Create("C3", 7.00m, SpaceType.Standard);
        var mockRepository = new Mock<IParkingSpacesRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(spaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(parkingSpace);

        var handler = new DeleteParkingSpaceCommandHandler(mockRepository.Object);
        var command = new DeleteParkingSpaceCommand(spaceId);

        await handler.Handle(command, CancellationToken.None);

        mockRepository.Verify(r => r.DeleteAsync(spaceId, It.IsAny<CancellationToken>()), Times.Once);
    }
}