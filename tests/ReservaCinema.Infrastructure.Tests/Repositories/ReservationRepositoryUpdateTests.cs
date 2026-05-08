using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ReservaCinema.Infrastructure.Persistence;
using ReservaCinema.Infrastructure.Persistence.Repositories;
using ReservaCinema.Tests.Shared.Builders;

namespace ReservaCinema.Infrastructure.Tests.Repositories;

public class ReservationRepositoryUpdateTests
{
    private static ReservaCinemaDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ReservaCinemaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ReservaCinemaDbContext(options);
    }

    [Fact]
    public async Task UpdateAsync_ComReservaExistente_DeveAtualizarCampos()
    {
        await using var context = CreateDbContext();
        var repository = new ReservationRepository(context);
        var reservation = new ReservationBuilder().Build();
        await repository.AddAsync(reservation);

        reservation.Status = "confirmed";
        reservation.SaleId = "sale-abc";
        reservation.PaidAt = DateTime.UtcNow;
        reservation.PaymentMethod = "credit_card";
        reservation.TransactionId = "tx-456";

        await repository.UpdateAsync(reservation);

        var updated = await context.Reservations.FirstOrDefaultAsync(r => r.Id == reservation.Id);
        updated!.Status.Should().Be("confirmed");
        updated.SaleId.Should().Be("sale-abc");
        updated.PaidAt.Should().NotBeNull();
        updated.PaymentMethod.Should().Be("credit_card");
        updated.TransactionId.Should().Be("tx-456");
    }

    [Fact]
    public async Task UpdateAsync_DeveRetornarReservaAtualizada()
    {
        await using var context = CreateDbContext();
        var repository = new ReservationRepository(context);
        var reservation = new ReservationBuilder().Build();
        await repository.AddAsync(reservation);

        reservation.Status = "confirmed";
        var result = await repository.UpdateAsync(reservation);

        result.Status.Should().Be("confirmed");
    }
}
