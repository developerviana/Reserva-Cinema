using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ReservaCinema.Application.Services;
using ReservaCinema.Infrastructure.Persistence;

namespace ReservaCinema.API.Tests.Setup;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Banco isolado por instância de factory — sem interferência entre classes de teste
            var dbName = $"TestDb_{Guid.NewGuid()}";
            var dbDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(DbContextOptions<ReservaCinemaDbContext>));
            if (dbDescriptor != null) services.Remove(dbDescriptor);
            services.AddDbContext<ReservaCinemaDbContext>((_, options) =>
                options.UseInMemoryDatabase(dbName));

            // Mock de IDistributedLockService — sem Redis real nos testes de integração
            var lockDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IDistributedLockService));
            if (lockDescriptor != null) services.Remove(lockDescriptor);

            var mockLock = new Mock<IDistributedLockService>();
            mockLock.Setup(x => x.AcquireLockAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()))
                .ReturnsAsync(Guid.NewGuid().ToString());
            mockLock.Setup(x => x.ReleaseLockAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);
            mockLock.Setup(x => x.IsLockOwnedAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            services.AddScoped(_ => mockLock.Object);
        });

        builder.UseEnvironment("Test");
    }
}
