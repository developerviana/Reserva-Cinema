using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ReservaCinema.Application.Services;
using ReservaCinema.Application.Services.Interfaces;
using ReservaCinema.Application.Validators.Sessions;

namespace ReservaCinema.Application;

/// <summary>
/// Extensões para registrar serviços da camada Application.
/// </summary>
public static class ApplicationExtensions
{
    /// <summary>
    /// Registra serviços de aplicação (Services, Validators).
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register Services
        services.AddScoped<ISessionService, SessionService>();

        // Register Validators
        services.AddValidatorsFromAssemblyContaining<CreateSessionRequestValidator>();

        return services;
    }
}
