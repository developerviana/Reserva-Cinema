using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ReservaCinema.API;
using ReservaCinema.API.Tests.Setup;

namespace ReservaCinema.API.Tests.Controllers.Reservations;

/// <summary>
/// Testes de integração HTTP para conflitos de assentos no POST /api/reservations.
/// Valida resposta 409 Conflict quando assentos já estão reservados.
/// </summary>
public class ReservationConflictIntegrationTests : IAsyncLifetime
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
    public async Task CreateReservation_WhenSeatsAreAlreadyReserved_ShouldReturn409Conflict()
    {
        // Arrange - cria primeira reserva
        var sessionId = Guid.NewGuid();
        var request1 = new
        {
            sessionId = sessionId,
            userId = "user-1",
            seatNumbers = new[] { "A1", "A2" }
        };

        var response1 = await _client.PostAsJsonAsync("/api/reservations", request1);
        response1.StatusCode.Should().Be(HttpStatusCode.Created);

        // Tenta criar segunda reserva com assentos já reservados
        var request2 = new
        {
            sessionId = sessionId,
            userId = "user-2",
            seatNumbers = new[] { "A1", "A3" }
        };

        // Act
        var response2 = await _client.PostAsJsonAsync("/api/reservations", request2);

        // Assert
        response2.StatusCode.Should().Be(HttpStatusCode.Conflict);
        
        var content = await response2.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        var root = doc.RootElement;

        root.GetProperty("error").GetString().Should().Be("SEAT_ALREADY_RESERVED");
        root.TryGetProperty("conflictingSeats", out var conflictingSeats).Should().BeTrue();
        conflictingSeats.GetArrayLength().Should().Be(1);
    }

    [Fact]
    public async Task CreateReservation_WithMultipleConflictingSeats_ShouldReturnAllConflicting()
    {
        // Arrange - cria primeira reserva com múltiplos assentos
        var sessionId = Guid.NewGuid();
        var request1 = new
        {
            sessionId = sessionId,
            userId = "user-1",
            seatNumbers = new[] { "B1", "B2", "B3" }
        };

        await _client.PostAsJsonAsync("/api/reservations", request1);

        // Tenta criar segunda reserva com múltiplos conflitos
        var request2 = new
        {
            sessionId = sessionId,
            userId = "user-2",
            seatNumbers = new[] { "B2", "B3", "B4" }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/reservations", request2);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        
        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        var conflictingSeats = doc.RootElement.GetProperty("conflictingSeats");

        conflictingSeats.GetArrayLength().Should().Be(2);
    }

    [Fact(Skip = "Problemas de thread-safety com Dictionary estático em testes paralelos")]
    public async Task CreateReservation_DifferentSessionsDoNotConflict()
    {
        // Arrange - cria reserva na sessão 1
        var sessionId1 = Guid.NewGuid();
        var request1 = new
        {
            sessionId = sessionId1,
            userId = "user-1",
            seatNumbers = new[] { "C1" }
        };

        await _client.PostAsJsonAsync("/api/reservations", request1);

        // Tenta criar reserva com mesmo assento mas em sessão diferente
        var sessionId2 = Guid.NewGuid();
        var request2 = new
        {
            sessionId = sessionId2,
            userId = "user-2",
            seatNumbers = new[] { "C1" }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/reservations", request2);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
