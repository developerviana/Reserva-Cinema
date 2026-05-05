using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ReservaCinema.Application.Services;
using ReservaCinema.Application.Services.Interfaces;
using ReservaCinema.Application.Validators.Sessions;

namespace ReservaCinema.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IReservationService, ReservationService>();

        services.AddValidatorsFromAssemblyContaining<CreateSessionRequestValidator>();

        return services;
    }
}
