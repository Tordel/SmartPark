using Microsoft.EntityFrameworkCore;
using SmartPark.Application.Abstractions;
using SmartPark.Domain.Entities;
using SmartPark.Infrastructure.Persistence;

namespace SmartPark.Infrastructure.Repositories;

public class ParkingSpacesRepository : IParkingSpacesRepository
{
    private readonly SmartParkDbContext _context;
    
    public ParkingSpacesRepository(SmartParkDbContext context)
    {
        _context = context;
    }

    public IQueryable<ParkingSpace> Query()
    {
        return _context.ParkingSpaces;
    }
    
    public async Task<ParkingSpace?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ParkingSpaces.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<ParkingSpace>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ParkingSpaces.ToListAsync(cancellationToken);
    }
    
    public async Task AddAsync(ParkingSpace parkingSpace, CancellationToken cancellationToken = default)
    {
        await _context.ParkingSpaces.AddAsync(parkingSpace, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task UpdateAsync(ParkingSpace parkingSpace, CancellationToken cancellationToken = default)
    {
        _context.ParkingSpaces.Update(parkingSpace);
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task DeleteAsync(Guid parkingSpaceId, CancellationToken cancellationToken = default)
    {
        var parkingSpace = await GetByIdAsync(parkingSpaceId, cancellationToken);
        
        if (parkingSpace == null)
            throw new InvalidOperationException($"Parking space {parkingSpaceId} not found.");
        
        _context.ParkingSpaces.Remove(parkingSpace);
        await _context.SaveChangesAsync(cancellationToken);
    }
}