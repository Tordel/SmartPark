using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartPark.Application.Abstractions;
using SmartPark.Infrastructure.Persistence;
using SmartPark.Infrastructure.Repositories;

namespace SmartPark.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SmartParkDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("SmartParkDb")));

        services.AddScoped<IParkingSpacesRepository, ParkingSpacesRepository>();
        services.AddScoped<IReservationsRepository, ReservationsRepository>();
        
        return services;
    }
}