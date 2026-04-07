using ReservaCinema.Application.DTOs.Reservations;
using ReservaCinema.Application.Services;

namespace ReservaCinema.Application.Tests.Services;

/// <summary>
/// Testes unitários para GetPurchaseHistoryAsync do ReservationService.
/// Testam regras de negócio de histórico de compras em isolamento.
/// </summary>
public class PurchaseHistoryTests
{
    private readonly ReservationService _service = new();

    [Fact]
    public async Task GetPurchaseHistory_WithEmptyUserId_ShouldThrowArgumentException()
    {
        // Act & Assert
        await FluentActions.Invoking(() => _service.GetPurchaseHistoryAsync(string.Empty))
            .Should()
            .ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetPurchaseHistory_WithWhitespaceUserId_ShouldThrowArgumentException()
    {
        // Act & Assert
        await FluentActions.Invoking(() => _service.GetPurchaseHistoryAsync("   "))
            .Should()
            .ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetPurchaseHistory_WhenUserHasNoPurchases_ShouldReturnEmptyList()
    {
        // Act
        var result = await _service.GetPurchaseHistoryAsync("user-no-purchases");

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be("user-no-purchases");
        result.Purchases.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPurchaseHistory_WhenUserHasPurchases_ShouldReturnAllPurchases()
    {
        // Arrange - cria uma reserva e a confirma
        var createRequest = new CreateReservationRequest
        {
            SessionId = Guid.NewGuid(),
            UserId = "user-with-purchases",
            SeatNumbers = new[] { "A1", "A2" }
        };

        var createdReservation = await _service.CreateReservationAsync(createRequest);

        var confirmRequest = new ConfirmPaymentRequest
        {
            PaymentMethod = "credit_card",
            TransactionId = "tx-111"
        };

        await _service.ConfirmPaymentAsync(createdReservation.ReservationId, confirmRequest);

        // Act - busca histórico de compras
        var result = await _service.GetPurchaseHistoryAsync("user-with-purchases");

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be("user-with-purchases");
        result.Purchases.Should().HaveCount(1);
        
        var purchase = result.Purchases.First();
        purchase.SaleId.Should().StartWith("sale-");
        purchase.Seats.Should().Equal("A1", "A2");
        purchase.TotalAmount.Should().Be(51.00m);
        purchase.PurchasedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public async Task GetPurchaseHistory_WithMultiplePurchases_ShouldReturnAllInOrder()
    {
        // Arrange - cria múltiplas reservas
        var userId = "user-multiple-purchases";

        for (int i = 1; i <= 3; i++)
        {
            var createRequest = new CreateReservationRequest
            {
                SessionId = Guid.NewGuid(),
                UserId = userId,
                SeatNumbers = new[] { $"A{i}" }
            };

            var createdReservation = await _service.CreateReservationAsync(createRequest);

            var confirmRequest = new ConfirmPaymentRequest
            {
                PaymentMethod = "credit_card",
                TransactionId = $"tx-{i}"
            };

            await _service.ConfirmPaymentAsync(createdReservation.ReservationId, confirmRequest);
        }

        // Act
        var result = await _service.GetPurchaseHistoryAsync(userId);

        // Assert
        result.Purchases.Should().HaveCount(3);
        result.Purchases.Should().AllSatisfy(p => p.SaleId.Should().StartWith("sale-"));
    }
}
