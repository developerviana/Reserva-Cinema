using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace ReservaCinema.API.Tests.Integration.Reservations;

/// <summary>
/// Testes de integração HTTP para POST /api/reservations endpoint.
/// Testa requisições reais contra o endpoint.
/// </summary>
public class CreateReservationUnitTests : IAsyncLifetime
{
    private HttpClient _httpClient = null!;

    public async Task InitializeAsync()
    {
        // Setup - Cria um cliente HTTP para testes
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5000")
        };
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _httpClient?.Dispose();
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
        var response = await _httpClient.PostAsJsonAsync("/api/reservations", request);

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
        var response = await _httpClient.PostAsJsonAsync("/api/reservations", request);

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
        var response = await _httpClient.PostAsJsonAsync("/api/reservations", request);

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
        var response = await _httpClient.PostAsJsonAsync("/api/reservations", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateReservation_WithSeatsAlreadyReserved_ShouldReturn409Conflict()
    {
        // Arrange - Simula assentos já reservados
        var sessionId = Guid.NewGuid();
        var request = new
        {
            sessionId = sessionId,
            userId = "user-123",
            seatNumbers = new[] { "A1" }
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/reservations", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var content = await response.Content.ReadFromJsonAsync<dynamic>();
        content?.error.Should().Be("SEAT_ALREADY_RESERVED");
    }
}

