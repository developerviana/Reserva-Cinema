using ReservaCinema.Domain.Entities;
using ReservaCinema.Domain.Exceptions;
using static FluentAssertions.FluentActions;

namespace ReservaCinema.Domain.Tests.Entities;

/// <summary>
/// Testes unitários para a entidade Reservation.
/// Testam as regras de negócio de confirmação de pagamento em isolamento.
/// </summary>
public class ReservationTests
{
    [Fact]
    public void ConfirmPayment_WhenReservationIsExpired_ShouldThrowReservationExpiredException()
    {
        // Arrange
        var expiredReservation = new Reservation
        {
            Id = "res-001",
            Status = "pending",
            ExpiresAt = DateTime.UtcNow.AddSeconds(-1)
        };

        // Act & Assert
        FluentActions.Invoking(() => expiredReservation.ConfirmPayment("tx-123"))
            .Should()
            .Throw<ReservationExpiredException>();
    }

    [Fact]
    public void ConfirmPayment_WhenReservationIsAlreadyConfirmed_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var confirmedReservation = new Reservation
        {
            Id = "res-002",
            Status = "confirmed",
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };

        // Act & Assert
        FluentActions.Invoking(() => confirmedReservation.ConfirmPayment("tx-123"))
            .Should()
            .Throw<InvalidOperationException>();
    }

    [Fact]
    public void ConfirmPayment_WhenReservationIsValidAndPending_ShouldUpdateStatusAndGenerateSaleId()
    {
        // Arrange
        var reservation = new Reservation
        {
            Id = "res-003",
            Status = "pending",
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            SeatsJson = "A1,A2",
            TotalAmount = 51.00m
        };

        // Act
        reservation.ConfirmPayment("tx-456");

        // Assert
        reservation.Status.Should().Be("confirmed");
        reservation.SaleId.Should().NotBeNullOrWhiteSpace();
        reservation.SaleId.Should().StartWith("sale-");
        reservation.PaidAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }
}
