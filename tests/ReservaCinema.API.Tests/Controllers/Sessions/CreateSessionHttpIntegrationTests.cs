using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ReservaCinema.API;
using ReservaCinema.API.Tests.Setup;

namespace ReservaCinema.API.Tests.Controllers.Sessions;

/// <summary>
/// Testes de integração HTTP para POST /api/sessions endpoint.
/// Testa requisições reais contra o endpoint usando WebApplicationFactory.
/// </summary>
public class CreateSessionHttpIntegrationTests : IAsyncLifetime
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
    public async Task CreateSession_WithValidData_ShouldReturn201Created()
    {
        // Arrange
        var futureTime = DateTime.UtcNow.AddHours(2);
        var request = new
        {
            movieTitle = "Oppenheimer",
            startTime = futureTime,
            roomNumber = "A1",
            totalSeats = 16,
            ticketPrice = 25.00
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/sessions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("\"movieTitle\":\"Oppenheimer\"");
        content.Should().Contain("\"availableSeats\":16");
        content.Should().Contain("\"id\":");
        content.Should().Contain("\"createdAt\":");
    }

    [Fact]
    public async Task CreateSession_WithEmptyMovieTitle_ShouldReturn400BadRequest()
    {
        // Arrange
        var futureTime = DateTime.UtcNow.AddHours(1);
        var request = new
        {
            movieTitle = "",
            startTime = futureTime,
            roomNumber = "A1",
            totalSeats = 100,
            ticketPrice = 25.00
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/sessions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateSession_WithPastStartTime_ShouldReturn400BadRequest()
    {
        // Arrange
        var pastTime = DateTime.UtcNow.AddHours(-1);
        var request = new
        {
            movieTitle = "Movie",
            startTime = pastTime,
            roomNumber = "A1",
            totalSeats = 100,
            ticketPrice = 25.00
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/sessions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateSession_WithEmptyRoomNumber_ShouldReturn400BadRequest()
    {
        // Arrange
        var futureTime = DateTime.UtcNow.AddHours(1);
        var request = new
        {
            movieTitle = "Movie",
            startTime = futureTime,
            roomNumber = "",
            totalSeats = 100,
            ticketPrice = 25.00
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/sessions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateSession_WithZeroSeats_ShouldReturn400BadRequest()
    {
        // Arrange
        var futureTime = DateTime.UtcNow.AddHours(1);
        var request = new
        {
            movieTitle = "Movie",
            startTime = futureTime,
            roomNumber = "A1",
            totalSeats = 0,
            ticketPrice = 25.00
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/sessions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateSession_WithNegativeTicketPrice_ShouldReturn400BadRequest()
    {
        // Arrange
        var futureTime = DateTime.UtcNow.AddHours(1);
        var request = new
        {
            movieTitle = "Movie",
            startTime = futureTime,
            roomNumber = "A1",
            totalSeats = 100,
            ticketPrice = -25.00
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/sessions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateSession_WithValidDataMultipleTimes_ShouldCreateDifferentSessions()
    {
        // Arrange
        var futureTime = DateTime.UtcNow.AddHours(1);
        var request = new
        {
            movieTitle = "Movie",
            startTime = futureTime,
            roomNumber = "A1",
            totalSeats = 100,
            ticketPrice = 25.00
        };

        // Act
        var response1 = await _client.PostAsJsonAsync("/api/sessions", request);
        var response2 = await _client.PostAsJsonAsync("/api/sessions", request);

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.Created);
        response2.StatusCode.Should().Be(HttpStatusCode.Created);

        var content1 = await response1.Content.ReadAsStringAsync();
        var content2 = await response2.Content.ReadAsStringAsync();

        using var doc1 = JsonDocument.Parse(content1);
        using var doc2 = JsonDocument.Parse(content2);

        var sessionId1 = doc1.RootElement.GetProperty("id").GetString();
        var sessionId2 = doc2.RootElement.GetProperty("id").GetString();

        sessionId1.Should().NotBe(sessionId2);
    }

    [Fact]
    public async Task CreateSession_ResponseShouldContainAllRequiredFields()
    {
        // Arrange
        var futureTime = DateTime.UtcNow.AddHours(2);
        var request = new
        {
            movieTitle = "Test Movie",
            startTime = futureTime,
            roomNumber = "B1",
            totalSeats = 50,
            ticketPrice = 30.00
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/sessions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("\"id\":");
        content.Should().Contain("\"movieTitle\":");
        content.Should().Contain("\"availableSeats\":");
        content.Should().Contain("\"createdAt\":");
    }
}
