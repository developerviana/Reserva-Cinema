using FluentAssertions;
using ReservaCinema.Application.DTOs.Reservations;
using ReservaCinema.Application.Services;

namespace ReservaCinema.Application.Tests.Services;

/// <summary>
/// Testes unitários para ReservationService.
/// Testam regras de negócio em isolamento (sem banco de dados).
/// </summary>
public class ReservationServiceTests
{
    private readonly ReservationService _service = new();

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateReservation_WithInvalidInput_ShouldThrowArgumentException(string userId)
    {
        var request = new CreateReservationRequest { SessionId = Guid.NewGuid(), UserId = userId, SeatNumbers = new[] { "A1" } };
        await FluentActions.Invoking(() => _service.CreateReservationAsync(request)).Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateReservation_WithEmptySessionId_ShouldThrowArgumentException()
    {
        var request = new CreateReservationRequest { SessionId = Guid.Empty, UserId = "user-1", SeatNumbers = new[] { "A1" } };
        await FluentActions.Invoking(() => _service.CreateReservationAsync(request)).Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateReservation_WithEmptySeats_ShouldThrowArgumentException()
    {
        var request = new CreateReservationRequest { SessionId = Guid.NewGuid(), UserId = "user-1", SeatNumbers = Array.Empty<string>() };
        await FluentActions.Invoking(() => _service.CreateReservationAsync(request)).Should().ThrowAsync<ArgumentException>();
    }

    [Theory]
    [InlineData(1, 25.50)]
    [InlineData(2, 51.00)]
    [InlineData(3, 76.50)]
    [InlineData(5, 127.50)]
    public async Task CreateReservation_ShouldCalculatePriceCorrectly(int seatCount, decimal expectedPrice)
    {
        var seats = Enumerable.Range(1, seatCount).Select(i => $"A{i}").ToArray();
        var request = new CreateReservationRequest { SessionId = Guid.NewGuid(), UserId = "user-1", SeatNumbers = seats };
        var response = await _service.CreateReservationAsync(request);
        response.TotalAmount.Should().Be(expectedPrice);
    }

    [Fact]
    public async Task CreateReservation_ShouldReturnValidResponse()
    {
        var seats = new[] { "A1", "A2" };
        var request = new CreateReservationRequest { SessionId = Guid.NewGuid(), UserId = "user-1", SeatNumbers = seats };
        var response = await _service.CreateReservationAsync(request);

        response.ReservationId.Should().StartWith("res-").And.HaveLength(12);
        response.Status.Should().Be("pending");
        response.Seats.Should().BeEquivalentTo(seats);
        response.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddHours(1), TimeSpan.FromSeconds(2));
    }

    // ===== RED TESTS - Não implementados ainda =====

    [Fact]
    public async Task CreateReservation_WithDuplicateSeats_ShouldThrowArgumentException()
    {
        var request = new CreateReservationRequest { SessionId = Guid.NewGuid(), UserId = "user-1", SeatNumbers = new[] { "A1", "A2", "A1" } };
        await FluentActions.Invoking(() => _service.CreateReservationAsync(request)).Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateReservation_WithMoreThan10Seats_ShouldThrowArgumentException()
    {
        var seats = Enumerable.Range(1, 11).Select(i => $"A{i}").ToArray();
        var request = new CreateReservationRequest { SessionId = Guid.NewGuid(), UserId = "user-1", SeatNumbers = seats };
        await FluentActions.Invoking(() => _service.CreateReservationAsync(request)).Should().ThrowAsync<ArgumentException>();
    }

    // ===== PAYMENT CONFIRMATION TESTS =====

    [Fact]
    public async Task ConfirmPayment_WhenReservationIdIsEmpty_ShouldThrowArgumentException()
    {
        var request = new ConfirmPaymentRequest { PaymentMethod = "credit_card", TransactionId = "tx-123" };
        await FluentActions.Invoking(() => _service.ConfirmPaymentAsync(string.Empty, request))
            .Should()
            .ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ConfirmPayment_WhenReservationIdIsInvalid_ShouldThrowArgumentException()
    {
        var request = new ConfirmPaymentRequest { PaymentMethod = "credit_card", TransactionId = "tx-123" };
        await FluentActions.Invoking(() => _service.ConfirmPaymentAsync("invalid-id", request))
            .Should()
            .ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ConfirmPayment_WhenPaymentMethodIsEmpty_ShouldThrowArgumentException()
    {
        var request = new ConfirmPaymentRequest { PaymentMethod = string.Empty, TransactionId = "tx-123" };
        await FluentActions.Invoking(() => _service.ConfirmPaymentAsync("res-001", request))
            .Should()
            .ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ConfirmPayment_WhenTransactionIdIsEmpty_ShouldThrowArgumentException()
    {
        var request = new ConfirmPaymentRequest { PaymentMethod = "credit_card", TransactionId = string.Empty };
        await FluentActions.Invoking(() => _service.ConfirmPaymentAsync("res-001", request))
            .Should()
            .ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ConfirmPayment_WhenReservationNotFound_ShouldReturnNull()
    {
        var request = new ConfirmPaymentRequest { PaymentMethod = "credit_card", TransactionId = "tx-123" };
        var result = await _service.ConfirmPaymentAsync("res-nonexistent", request);
        result.Should().BeNull();
    }

    [Fact]
    public async Task ConfirmPayment_WithValidRequest_ShouldReturnConfirmationResponse()
    {
        // Arrange - cria uma reserva válida
        var createRequest = new CreateReservationRequest
        {
            SessionId = Guid.NewGuid(),
            UserId = "user-valid",
            SeatNumbers = new[] { "A1", "A2" }
        };

        var createResponse = await _service.CreateReservationAsync(createRequest);
        var reservationId = createResponse.ReservationId;

        var confirmRequest = new ConfirmPaymentRequest
        {
            PaymentMethod = "credit_card",
            TransactionId = "tx-456"
        };

        // Act
        var response = await _service.ConfirmPaymentAsync(reservationId, confirmRequest);

        // Assert
        response.Should().NotBeNull();
        response!.Status.Should().Be("confirmed");
        response.SaleId.Should().StartWith("sale-");
        response.PaidAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }
}
