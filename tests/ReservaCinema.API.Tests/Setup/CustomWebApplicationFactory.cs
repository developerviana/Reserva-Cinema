using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ReservaCinema.Infrastructure.Persistence;

namespace ReservaCinema.API.Tests.Setup;

/// <summary>
/// Factory customizada para criar instâncias da aplicação para testes de integração.
/// </summary>
public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove a configuração de DbContext existente
            var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(DbContextOptions<ReservaCinemaDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Adiciona um banco em memória para os testes
            services.AddDbContext<ReservaCinemaDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("ReservaCinemaTestDb");
            });
        });

        builder.UseEnvironment("Test");
    }
}
