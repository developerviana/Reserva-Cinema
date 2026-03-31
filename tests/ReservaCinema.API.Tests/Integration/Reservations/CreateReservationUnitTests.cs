using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using ReservaCinema.API;
using ReservaCinema.API.Tests.Integration.Setup;

namespace ReservaCinema.API.Tests.Integration.Reservations;

/// <summary>
/// Testes de integração HTTP para POST /api/reservations endpoint.
/// Testa requisições reais contra o endpoint usando WebApplicationFactory.
/// </summary>
public class CreateReservationUnitTests : IAsyncLifetime
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
    public async Task CreateReservation_WithValidData_ShouldReturn201Created()
    {
        // Arrange
        var request = new
        {
            sessionId = Guid.NewGuid(),
            userId = "user-123",
            seatNumbers = new[] { "A1", "A2" }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/reservations", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateReservation_WithInvalidSessionId_ShouldReturn400BadRequest()
    {
        // Arrange
        var request = new
        {
            sessionId = Guid.Empty,
            userId = "user-123",
            seatNumbers = new[] { "A1" }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/reservations", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateReservation_WithEmptySeats_ShouldReturn400BadRequest()
    {
        // Arrange
        var request = new
        {
            sessionId = Guid.NewGuid(),
            userId = "user-123",
            seatNumbers = Array.Empty<string>()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/reservations", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateReservation_WithEmptyUserId_ShouldReturn400BadRequest()
    {
        // Arrange
        var request = new
        {
            sessionId = Guid.NewGuid(),
            userId = string.Empty,
            seatNumbers = new[] { "A1" }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/reservations", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateReservation_ShouldReturnCreatedResponse_WithAllRequiredFields()
    {
        // Arrange
        var request = new
        {
            sessionId = Guid.NewGuid(),
            userId = "user-456",
            seatNumbers = new[] { "B1", "B2" }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/reservations", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("reservationId");
        content.Should().Contain("pending");
        content.Should().Contain("expiresAt");
        content.Should().Contain("totalAmount");
    }
}



