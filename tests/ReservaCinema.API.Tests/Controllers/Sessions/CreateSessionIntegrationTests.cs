using System.Net;
using System.Net.Http.Json;
using ReservaCinema.API.Tests.Setup;
using ReservaCinema.Tests.Shared.Builders;

namespace ReservaCinema.API.Tests.Controllers.Sessions;

public class CreateSessionIntegrationTests : IAsyncLifetime
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
        _client.Dispose();
        await _factory.DisposeAsync();
    }

    [Fact]
    public async Task CreateSession_ComDadosValidos_DeveRetornar201()
    {
        // Arrange
        var request = new CreateSessionRequestBuilder().Build();

        // Act
        var response = await _client.PostAsJsonAsync("/api/sessions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateSession_ComTituloVazio_DeveRetornar400()
    {
        // Arrange
        var request = new CreateSessionRequestBuilder().WithMovieTitle("").Build();

        // Act
        var response = await _client.PostAsJsonAsync("/api/sessions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateSession_ComDataNoPassado_DeveRetornar400()
    {
        // Arrange
        var request = new CreateSessionRequestBuilder().WithStartTime(DateTime.UtcNow.AddHours(-1)).Build();

        // Act
        var response = await _client.PostAsJsonAsync("/api/sessions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateSession_ComSalaInvalida_DeveRetornar400()
    {
        // Arrange
        var request = new CreateSessionRequestBuilder().WithRoomNumber("A").Build();

        // Act
        var response = await _client.PostAsJsonAsync("/api/sessions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateSession_ComZeroAssentos_DeveRetornar400()
    {
        // Arrange
        var request = new CreateSessionRequestBuilder().WithTotalSeats(0).Build();

        // Act
        var response = await _client.PostAsJsonAsync("/api/sessions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateSession_ComPrecoNegativo_DeveRetornar400()
    {
        // Arrange
        var request = new CreateSessionRequestBuilder().WithTicketPrice(-1m).Build();

        // Act
        var response = await _client.PostAsJsonAsync("/api/sessions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateSession_DeveRetornarCorpoComCamposEsperados()
    {
        // Arrange
        var request = new CreateSessionRequestBuilder().WithMovieTitle("Inception").Build();

        // Act
        var response = await _client.PostAsJsonAsync("/api/sessions", request);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        content.Should().Contain("id");
        content.Should().Contain("movieTitle");
        content.Should().Contain("Inception");
    }
}
