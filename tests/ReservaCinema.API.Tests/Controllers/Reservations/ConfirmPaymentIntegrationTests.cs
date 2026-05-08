using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ReservaCinema.API;
using ReservaCinema.API.Tests.Setup;

namespace ReservaCinema.API.Tests.Controllers.Reservations;

/// <summary>
/// Testes de integração HTTP para POST /api/reservations/{id}/confirm endpoint.
/// Contrato: 200 OK (confirmado), 400 Bad Request (input inválido),
///            404 Not Found (reserva inexistente), 410 Gone (reserva expirada).
/// </summary>
public class ConfirmPaymentIntegrationTests : IAsyncLifetime
{
    private CustomWebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        _factory = new CustomWebApplicationFactory<Program>();
        _client = _factory.CreateClient();
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        _factory?.Dispose();
        await Task.CompletedTask;
    }

    private async Task<Guid> CreateSessionAsync()
    {
        var sessionRequest = new
        {
            movieTitle = "Filme Teste",
            startTime = DateTime.UtcNow.AddHours(2),
            roomNumber = "A1",
            totalSeats = 100,
            ticketPrice = 30.00m
        };
        var response = await _client.PostAsJsonAsync("/api/sessions", sessionRequest);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        return json.RootElement.GetProperty("id").GetGuid();
    }

    private async Task<string> CreateReservationAsync(Guid sessionId)
    {
        var request = new { sessionId, userId = "user-123", seatNumbers = new[] { "A1", "A2" } };
        var response = await _client.PostAsJsonAsync("/api/reservations", request);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        return json.RootElement.GetProperty("reservationId").GetString()!;
    }

    [Fact]
    public async Task ConfirmPayment_ComDadosValidos_ShouldReturn200OK()
    {
        // Arrange
        var sessionId = await CreateSessionAsync();
        var reservationId = await CreateReservationAsync(sessionId);
        var request = new { paymentMethod = "credit_card", transactionId = "tx-456" };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/reservations/{reservationId}/confirm", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ConfirmPayment_ComDadosValidos_ShouldReturnAllRequiredFields()
    {
        // Arrange
        var sessionId = await CreateSessionAsync();
        var reservationId = await CreateReservationAsync(sessionId);
        var request = new { paymentMethod = "credit_card", transactionId = "tx-456" };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/reservations/{reservationId}/confirm", request);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().Contain("saleId");
        content.Should().Contain("confirmed");
        content.Should().Contain("seats");
        content.Should().Contain("paidAt");
    }

    [Fact]
    public async Task ConfirmPayment_ComPaymentMethodVazio_ShouldReturn400BadRequest()
    {
        // Arrange
        var request = new { paymentMethod = "", transactionId = "tx-456" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/reservations/any-id/confirm", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ConfirmPayment_ComTransactionIdVazio_ShouldReturn400BadRequest()
    {
        // Arrange
        var request = new { paymentMethod = "credit_card", transactionId = "" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/reservations/any-id/confirm", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ConfirmPayment_ComReservaInexistente_ShouldReturn404NotFound()
    {
        // Arrange
        var request = new { paymentMethod = "credit_card", transactionId = "tx-456" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/reservations/reserva-inexistente/confirm", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ConfirmPayment_ComReservaExpirada_ShouldReturn410Gone()
    {
        // Arrange — reserva expirada criada diretamente via factory
        var reservationId = await _factory.CreateExpiredReservationAsync();
        var request = new { paymentMethod = "credit_card", transactionId = "tx-456" };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/reservations/{reservationId}/confirm", request);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Gone);
        content.Should().Contain("RESERVATION_EXPIRED");
    }
}
