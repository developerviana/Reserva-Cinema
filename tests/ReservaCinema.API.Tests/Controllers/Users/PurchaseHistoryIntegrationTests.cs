using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ReservaCinema.API;
using ReservaCinema.API.Tests.Setup;

namespace ReservaCinema.API.Tests.Controllers.Users;

/// <summary>
/// Testes de integração HTTP para GET /api/users/{userId}/purchases endpoint.
/// Testa requisições reais contra o endpoint usando WebApplicationFactory.
/// </summary>
public class PurchaseHistoryIntegrationTests : IAsyncLifetime
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
    public async Task GetPurchaseHistory_WithValidUserId_ShouldReturn200Ok()
    {
        // Arrange - cria e confirma uma reserva
        var createRequest = new
        {
            sessionId = Guid.NewGuid(),
            userId = "user-integration-test",
            seatNumbers = new[] { "A1", "A2" }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/reservations", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdContent = await createResponse.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(createdContent);
        var reservationId = doc.RootElement.GetProperty("reservationId").GetString();

        // Confirma o pagamento
        var confirmRequest = new
        {
            paymentMethod = "credit_card",
            transactionId = "tx-integration-111"
        };

        var confirmResponse = await _client.PostAsJsonAsync($"/api/reservations/{reservationId}/confirm", confirmRequest);
        confirmResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act - busca histórico de compras
        var historyResponse = await _client.GetAsync("/api/users/user-integration-test/purchases");

        // Assert
        historyResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var historyContent = await historyResponse.Content.ReadAsStringAsync();
        historyContent.Should().Contain("user-integration-test");
        historyContent.Should().Contain("sale-");
        historyContent.Should().Contain("\"seats\":");
    }

    [Fact]
    public async Task GetPurchaseHistory_WithUserWithoutPurchases_ShouldReturnEmptyList()
    {
        // Act
        var response = await _client.GetAsync("/api/users/user-no-purchases/purchases");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        var root = doc.RootElement;

        root.GetProperty("userId").GetString().Should().Be("user-no-purchases");
        var purchases = root.GetProperty("purchases");
        purchases.GetArrayLength().Should().Be(0);
    }

    [Fact]
    public async Task GetPurchaseHistory_WithMultiplePurchases_ShouldReturnAllPurchases()
    {
        // Arrange - cria múltiplas reservas e confirma
        var userId = "user-multiple-integration";

        for (int i = 1; i <= 2; i++)
        {
            var createRequest = new
            {
                sessionId = Guid.NewGuid(),
                userId = userId,
                seatNumbers = new[] { $"A{i}" }
            };

            var createResponse = await _client.PostAsJsonAsync("/api/reservations", createRequest);
            var createdContent = await createResponse.Content.ReadAsStringAsync();
            using var createDoc = JsonDocument.Parse(createdContent);
            var reservationId = createDoc.RootElement.GetProperty("reservationId").GetString();

            var confirmRequest = new
            {
                paymentMethod = "credit_card",
                transactionId = $"tx-multi-{i}"
            };

            await _client.PostAsJsonAsync($"/api/reservations/{reservationId}/confirm", confirmRequest);
        }

        // Act
        var response = await _client.GetAsync($"/api/users/{userId}/purchases");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        var purchases = doc.RootElement.GetProperty("purchases");

        purchases.GetArrayLength().Should().Be(2);
    }

    [Fact]
    public async Task GetPurchaseHistory_WithInvalidUserId_ShouldReturnEmptyList()
    {
        // Act
        var response = await _client.GetAsync("/api/users/invalid-user-format/purchases");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        var purchases = doc.RootElement.GetProperty("purchases");
        purchases.GetArrayLength().Should().Be(0);
    }
}
