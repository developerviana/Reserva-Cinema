using System.Net;
using System.Net.Http.Json;
using ReservaCinema.API.Tests.Setup;
using ReservaCinema.Application.DTOs.Sessions;
using ReservaCinema.Tests.Shared.Builders;

namespace ReservaCinema.API.Tests.Controllers.Sessions;

public class GetSessionIntegrationTests : IAsyncLifetime
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
    public async Task GetSessionById_ComSessaoExistente_DeveRetornar200ComDados()
    {
        // Arrange — cria sessão via POST para garantir estado real no banco
        var createRequest = new CreateSessionRequestBuilder().WithMovieTitle("Matrix").Build();
        var createResponse = await _client.PostAsJsonAsync("/api/sessions", createRequest);
        createResponse.EnsureSuccessStatusCode();

        var created = await createResponse.Content.ReadFromJsonAsync<SessionResponse>();
        var sessionId = created!.Id;

        // Act
        var response = await _client.GetAsync($"/api/sessions/{sessionId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Matrix");
    }

    [Fact]
    public async Task GetSessionById_ComIdInexistente_DeveRetornar404()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/sessions/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
