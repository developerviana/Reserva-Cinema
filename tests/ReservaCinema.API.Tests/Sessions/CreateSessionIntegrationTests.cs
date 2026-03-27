using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ReservaCinema.Application.DTOs.Sessions;
using ReservaCinema.Application.Persistence;
using System.Net;
using System.Text;
using System.Text.Json;

namespace ReservaCinema.API.Tests.Sessions;

public class CreateSessionIntegrationTests : IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private HttpClient _httpClient = null!;
    private ReservaCinemaDbContext _dbContext = null!;

    public CreateSessionIntegrationTests()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ReservaCinemaDbContext>));

                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<ReservaCinemaDbContext>(options =>
                        options.UseInMemoryDatabase("cinema_test_db"));
                });
            });
    }

    public async Task InitializeAsync()
    {
        _httpClient = _factory.CreateClient();
        
        using (var scope = _factory.Services.CreateScope())
        {
            _dbContext = scope.ServiceProvider.GetRequiredService<ReservaCinemaDbContext>();
            await _dbContext.Database.EnsureCreatedAsync();
        }
    }

    public async Task DisposeAsync()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        _factory.Dispose();
        _httpClient.Dispose();
    }

    [Fact]
    public async Task PostCreateSession_ShouldReturnCreated_WhenValidDataProvided()
    {
        // Arrange
        var request = new CreateSessionRequest
        {
            MovieTitle = "The Dark Knight",
            StartTime = DateTime.UtcNow.AddHours(2),
            RoomNumber = "A1",
            TotalSeats = 100,
            TicketPrice = 25.50
        };

        var content = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _httpClient.PostAsync("/api/sessions", content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ReservaCinemaDbContext>();
            var savedSession = await context.Sessions.FirstOrDefaultAsync(s => s.MovieTitle == "The Dark Knight");
            Assert.NotNull(savedSession);
            Assert.Equal("A1", savedSession.RoomNumber);
        }
    }

    [Fact]
    public async Task PostCreateSession_ShouldReturnBadRequest_WhenInvalidMovieTitle()
    {
        // Arrange
        var request = new CreateSessionRequest
        {
            MovieTitle = null,
            StartTime = DateTime.UtcNow.AddHours(2),
            RoomNumber = "A1",
            TotalSeats = 100,
            TicketPrice = 25.50
        };

        var content = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _httpClient.PostAsync("/api/sessions", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostCreateSession_ShouldReturnBadRequest_WhenPastStartTime()
    {
        // Arrange
        var request = new CreateSessionRequest
        {
            MovieTitle = "Interstellar",
            StartTime = DateTime.UtcNow.AddHours(-1), // Past time
            RoomNumber = "B2",
            TotalSeats = 100,
            TicketPrice = 30.00
        };

        var content = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _httpClient.PostAsync("/api/sessions", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostCreateSession_ShouldReturnBadRequest_WhenInvalidRoomNumber()
    {
        // Arrange
        var request = new CreateSessionRequest
        {
            MovieTitle = "Inception",
            StartTime = DateTime.UtcNow.AddHours(2),
            RoomNumber = "A", // Less than 2 characters
            TotalSeats = 100,
            TicketPrice = 30.00
        };

        var content = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _httpClient.PostAsync("/api/sessions", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostCreateSession_ShouldReturnBadRequest_WhenZeroTotalSeats()
    {
        // Arrange
        var request = new CreateSessionRequest
        {
            MovieTitle = "Avatar",
            StartTime = DateTime.UtcNow.AddHours(2),
            RoomNumber = "C3",
            TotalSeats = 0, // Invalid
            TicketPrice = 35.00
        };

        var content = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _httpClient.PostAsync("/api/sessions", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostCreateSession_ShouldPersistToDatabase_WhenValidSessionCreated()
    {
        // Arrange
        var request = new CreateSessionRequest
        {
            MovieTitle = "Oppenheimer",
            StartTime = DateTime.UtcNow.AddHours(3),
            RoomNumber = "D4",
            TotalSeats = 120,
            TicketPrice = 32.50
        };

        var content = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _httpClient.PostAsync("/api/sessions", content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ReservaCinemaDbContext>();
            var sessionsCount = await context.Sessions.CountAsync();
            Assert.Equal(1, sessionsCount);

            var savedSession = await context.Sessions.FirstAsync();
            Assert.Equal("Oppenheimer", savedSession.MovieTitle);
            Assert.Equal("D4", savedSession.RoomNumber);
            Assert.Equal(120, savedSession.TotalSeats);
        }
    }
}
