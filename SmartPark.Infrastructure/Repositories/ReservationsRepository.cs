using Microsoft.EntityFrameworkCore;
using SmartPark.Application.Abstractions;
using SmartPark.Domain.Entities;
using SmartPark.Infrastructure.Persistence;

namespace SmartPark.Infrastructure.Repositories;

public class ReservationsRepository : IReservationsRepository
{
    private readonly SmartParkDbContext _context;
    
    public ReservationsRepository(SmartParkDbContext context)
    {
        _context = context;
    }

    public async Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Reservations.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<Reservation>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (Guid.TryParse(userId, out var customerId))
        {
            return await _context.Reservations
                .Where(r => r.CustomerId == customerId)
                .ToListAsync(cancellationToken);
        }
        
        return Enumerable.Empty<Reservation>();
    }

    public async Task<IEnumerable<Reservation>> GetByParkingSpaceIdAsync(Guid parkingSpaceId, CancellationToken cancellationToken = default)
    {
        return await _context.Reservations
            .Where(r => r.ParkingSpaceId == parkingSpaceId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Reservation>> GetActiveReservationsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Reservations
            .Where(r => r.Interval.Start <= DateTime.UtcNow && r.Interval.End >= DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Reservation>> GetExpiredReservationsAsync(DateTime cutoffDate, CancellationToken cancellationToken = default)
    {
        return await _context.Reservations
            .Where(r => r.Interval.End <= cutoffDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Reservation>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Reservations.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        await _context.Reservations.AddAsync(reservation, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        _context.Reservations.Update(reservation);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var reservation = await GetByIdAsync(id, cancellationToken);
        if (reservation != null)
        {
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            throw new InvalidOperationException($"Reservation {id} not found.");
        }
    }
}