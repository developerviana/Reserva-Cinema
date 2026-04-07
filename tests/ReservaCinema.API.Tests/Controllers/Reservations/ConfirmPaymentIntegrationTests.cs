using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ReservaCinema.API;
using ReservaCinema.API.Tests.Setup;

namespace ReservaCinema.API.Tests.Controllers.Reservations;

/// <summary>
/// Testes de integração HTTP para POST /api/reservations/{id}/confirm endpoint.
/// Testa requisições reais contra o endpoint usando WebApplicationFactory.
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

    [Fact]
    public async Task ConfirmPayment_WithValidReservationAndPayment_ShouldReturn200Ok()
    {
        // Arrange - cria uma reserva primeiro
        var createRequest = new
        {
            sessionId = Guid.NewGuid(),
            userId = "user-123",
            seatNumbers = new[] { "A1", "A2" }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/reservations", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdContent = await createResponse.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(createdContent);
        var reservationId = doc.RootElement.GetProperty("reservationId").GetString();

        // Arrange - prepara confirmação de pagamento
        var confirmRequest = new
        {
            paymentMethod = "credit_card",
            transactionId = "tx-456"
        };

        // Act
        var confirmResponse = await _client.PostAsJsonAsync($"/api/reservations/{reservationId}/confirm", confirmRequest);

        // Assert
        confirmResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var confirmContent = await confirmResponse.Content.ReadAsStringAsync();
        confirmContent.Should().Contain("\"status\":\"confirmed\"");
        confirmContent.Should().Contain("\"saleId\":\"sale-");
        confirmContent.Should().Contain("\"paidAt\":");
    }

    [Fact]
    public async Task ConfirmPayment_WithInvalidReservationId_ShouldReturn404NotFound()
    {
        // Arrange
        var confirmRequest = new
        {
            paymentMethod = "credit_card",
            transactionId = "tx-123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/reservations/res-nonexistent/confirm", confirmRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ConfirmPayment_WithEmptyPaymentMethod_ShouldReturn400BadRequest()
    {
        // Arrange - cria uma reserva primeiro
        var createRequest = new
        {
            sessionId = Guid.NewGuid(),
            userId = "user-123",
            seatNumbers = new[] { "A1" }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/reservations", createRequest);
        var createdContent = await createResponse.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(createdContent);
        var reservationId = doc.RootElement.GetProperty("reservationId").GetString();

        // Arrange - confirmação com campo vazio
        var confirmRequest = new
        {
            paymentMethod = "",
            transactionId = "tx-123"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/reservations/{reservationId}/confirm", confirmRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
