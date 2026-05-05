using Microsoft.EntityFrameworkCore;
using ReservaCinema.Infrastructure.Persistence;
using ReservaCinema.Infrastructure.Persistence.Repositories;
using ReservaCinema.Tests.Shared.Builders;

namespace ReservaCinema.Infrastructure.Tests.Repositories;

public class ReservationRepositoryTests
{
    private static ReservaCinemaDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ReservaCinemaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ReservaCinemaDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ComReservaValida_DevePersistirERetornar()
    {
        await using var context = CreateDbContext();
        var repository = new ReservationRepository(context);
        var reservation = new ReservationBuilder().Build();

        var result = await repository.AddAsync(reservation);

        result.Should().NotBeNull();
        result.Id.Should().Be(reservation.Id);
        context.Reservations.Count().Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_ComIdExistente_DeveRetornarReserva()
    {
        await using var context = CreateDbContext();
        var repository = new ReservationRepository(context);
        var reservation = new ReservationBuilder().WithUserId("user-abc").Build();
        await repository.AddAsync(reservation);

        var result = await repository.GetByIdAsync(reservation.Id);

        result.Should().NotBeNull();
        result!.UserId.Should().Be("user-abc");
    }

    [Fact]
    public async Task GetByIdAsync_ComIdInexistente_DeveRetornarNull()
    {
        await using var context = CreateDbContext();
        var repository = new ReservationRepository(context);

        var result = await repository.GetByIdAsync(Guid.NewGuid().ToString());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetBySessionIdAsync_DeveRetornarApenasReservasDaSessao()
    {
        await using var context = CreateDbContext();
        var repository = new ReservationRepository(context);
        var sessionId = Guid.NewGuid();
        var outraSession = Guid.NewGuid();

        await repository.AddAsync(new ReservationBuilder().WithSessionId(sessionId).Build());
        await repository.AddAsync(new ReservationBuilder().WithSessionId(sessionId).Build());
        await repository.AddAsync(new ReservationBuilder().WithSessionId(outraSession).Build());

        var result = await repository.GetBySessionIdAsync(sessionId);

        result.Should().HaveCount(2);
        result.Should().AllSatisfy(r => r.SessionId.Should().Be(sessionId));
    }

    [Fact]
    public async Task GetBySessionIdAsync_NaoDeveRetornarReservasCanceladas()
    {
        await using var context = CreateDbContext();
        var repository = new ReservationRepository(context);
        var sessionId = Guid.NewGuid();

        await repository.AddAsync(new ReservationBuilder().WithSessionId(sessionId).WithStatus("pending").Build());
        await repository.AddAsync(new ReservationBuilder().WithSessionId(sessionId).WithStatus("cancelled").Build());

        var result = await repository.GetBySessionIdAsync(sessionId);

        result.Should().HaveCount(1);
        result.Single().Status.Should().Be("pending");
    }
}
