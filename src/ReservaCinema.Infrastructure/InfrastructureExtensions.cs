using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReservaCinema.Application.Persistence.Repositories;
using ReservaCinema.Infrastructure.Persistence;
using ReservaCinema.Infrastructure.Persistence.Repositories;

namespace ReservaCinema.Infrastructure;

/// <summary>
/// Extensões para registrar serviços da camada Infrastructure.
/// </summary>
public static class InfrastructureExtensions
{
    /// <summary>
    /// Registra serviços de infraestrutura (DbContext, Repositories).
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure PostgreSQL Database
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ReservaCinemaDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Register Repositories
        services.AddScoped<ISessionRepository, SessionRepository>();

        return services;
    }
}
