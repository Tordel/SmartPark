using SmartPark.Domain.Entities;

namespace SmartPark.Application.Abstractions;

public interface IParkingSpacesRepository
{
    Task<ParkingSpace?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ParkingSpace>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(ParkingSpace parkingSpace, CancellationToken cancellationToken = default);
    Task UpdateAsync(ParkingSpace parkingSpace, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    IQueryable<ParkingSpace> Query();
}