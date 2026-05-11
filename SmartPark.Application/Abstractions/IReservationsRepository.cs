using SmartPark.Domain.Entities;

namespace SmartPark.Application.Abstractions;

public interface IReservationsRepository
{
    Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Reservation>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Reservation>> GetByParkingSpaceIdAsync(Guid parkingSpaceId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Reservation>> GetActiveReservationsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Reservation>> GetExpiredReservationsAsync(DateTime cutoffDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<Reservation>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default);
    Task UpdateAsync(Reservation reservation, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}