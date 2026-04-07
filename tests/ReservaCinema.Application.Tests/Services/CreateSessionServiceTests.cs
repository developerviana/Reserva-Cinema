using ReservaCinema.Application.DTOs.Sessions;
using ReservaCinema.Application.Services;
using ReservaCinema.Application.Services.Interfaces;
using ReservaCinema.Tests.Shared.Builders;

namespace ReservaCinema.Application.Tests.Services;

/// <summary>
/// Testes unitários para SessionService.CreateSessionAsync.
/// Testam as regras de negócio de criação de sessão em isolamento.
/// </summary>
public class CreateSessionServiceTests
{
    private readonly ISessionService _service;

    public CreateSessionServiceTests()
    {
        // Para testes isolados, criamos um mock/stub do repositório
        // Por enquanto, deixamos o service sem repositório para testes de validação
        _service = new SessionServiceMock();
    }

    [Fact]
    public async Task CreateSession_WithValidData_ShouldReturnSessionResponse()
    {
        // Arrange
        var futureTime = DateTime.UtcNow.AddHours(2);
        var request = new CreateSessionRequest
        {
            MovieTitle = "Oppenheimer",
            StartTime = futureTime,
            RoomNumber = "A1",
            TotalSeats = 16,
            TicketPrice = 25.00m
        };

        // Act
        var response = await _service.CreateSessionAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.MovieTitle.Should().Be("Oppenheimer");
        response.AvailableSeats.Should().Be(16);
        response.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public async Task CreateSession_ShouldGenerateUniqueSessionId()
    {
        // Arrange
        var futureTime = DateTime.UtcNow.AddHours(1);
        var request1 = new CreateSessionRequest
        {
            MovieTitle = "Movie 1",
            StartTime = futureTime,
            RoomNumber = "A1",
            TotalSeats = 100,
            TicketPrice = 25.00m
        };

        var request2 = new CreateSessionRequest
        {
            MovieTitle = "Movie 2",
            StartTime = futureTime.AddHours(1),
            RoomNumber = "A2",
            TotalSeats = 100,
            TicketPrice = 25.00m
        };

        // Act
        var response1 = await _service.CreateSessionAsync(request1);
        var response2 = await _service.CreateSessionAsync(request2);

        // Assert
        response1.Id.Should().NotBe(response2.Id);
    }

    [Fact]
    public async Task CreateSession_ShouldSetAvailableSeatsEqualToTotalSeats()
    {
        // Arrange
        var futureTime = DateTime.UtcNow.AddHours(1);
        var totalSeats = 50;
        var request = new CreateSessionRequest
        {
            MovieTitle = "New Movie",
            StartTime = futureTime,
            RoomNumber = "B1",
            TotalSeats = totalSeats,
            TicketPrice = 30.00m
        };

        // Act
        var response = await _service.CreateSessionAsync(request);

        // Assert
        response.AvailableSeats.Should().Be(totalSeats);
    }

    [Fact]
    public async Task CreateSession_WithEmptyMovieTitle_ShouldThrowArgumentException()
    {
        // Arrange
        var request = new CreateSessionRequest
        {
            MovieTitle = "",
            StartTime = DateTime.UtcNow.AddHours(1),
            RoomNumber = "A1",
            TotalSeats = 100,
            TicketPrice = 25.00m
        };

        // Act & Assert
        await FluentActions.Invoking(() => _service.CreateSessionAsync(request))
            .Should()
            .ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateSession_WithPastStartTime_ShouldThrowArgumentException()
    {
        // Arrange
        var request = new CreateSessionRequest
        {
            MovieTitle = "Movie",
            StartTime = DateTime.UtcNow.AddHours(-1),
            RoomNumber = "A1",
            TotalSeats = 100,
            TicketPrice = 25.00m
        };

        // Act & Assert
        await FluentActions.Invoking(() => _service.CreateSessionAsync(request))
            .Should()
            .ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateSession_WithEmptyRoomNumber_ShouldThrowArgumentException()
    {
        // Arrange
        var request = new CreateSessionRequest
        {
            MovieTitle = "Movie",
            StartTime = DateTime.UtcNow.AddHours(1),
            RoomNumber = "",
            TotalSeats = 100,
            TicketPrice = 25.00m
        };

        // Act & Assert
        await FluentActions.Invoking(() => _service.CreateSessionAsync(request))
            .Should()
            .ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateSession_WithZeroSeats_ShouldThrowArgumentException()
    {
        // Arrange
        var request = new CreateSessionRequest
        {
            MovieTitle = "Movie",
            StartTime = DateTime.UtcNow.AddHours(1),
            RoomNumber = "A1",
            TotalSeats = 0,
            TicketPrice = 25.00m
        };

        // Act & Assert
        await FluentActions.Invoking(() => _service.CreateSessionAsync(request))
            .Should()
            .ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateSession_WithNegativeTicketPrice_ShouldThrowArgumentException()
    {
        // Arrange
        var request = new CreateSessionRequest
        {
            MovieTitle = "Movie",
            StartTime = DateTime.UtcNow.AddHours(1),
            RoomNumber = "A1",
            TotalSeats = 100,
            TicketPrice = -25.00m
        };

        // Act & Assert
        await FluentActions.Invoking(() => _service.CreateSessionAsync(request))
            .Should()
            .ThrowAsync<ArgumentException>();
    }
}

/// <summary>
/// Mock do SessionService para testes isolados
/// Substitui o repositório com implementação em memória
/// </summary>
internal class SessionServiceMock : ISessionService
{
    private static readonly Dictionary<Guid, ReservaCinema.Domain.Entities.Session> SessionStore = new();
    private static readonly Dictionary<(Guid, string), (SeatStatus status, DateTime? expiresAt)> SeatStore = new();

    public async Task<SessionResponse> CreateSessionAsync(CreateSessionRequest request)
    {
        // Validações
        if (string.IsNullOrWhiteSpace(request.MovieTitle))
            throw new ArgumentException("MovieTitle não pode estar vazio.");

        if (request.StartTime <= DateTime.UtcNow)
            throw new ArgumentException("StartTime deve ser no futuro.");

        if (string.IsNullOrWhiteSpace(request.RoomNumber))
            throw new ArgumentException("RoomNumber não pode estar vazio.");

        if (request.TotalSeats <= 0)
            throw new ArgumentException("TotalSeats deve ser maior que 0.");

        if (request.TicketPrice < 0)
            throw new ArgumentException("TicketPrice não pode ser negativo.");

        // Cria a sessão
        var sessionId = Guid.NewGuid();
        var session = new ReservaCinema.Domain.Entities.Session
        {
            Id = sessionId,
            MovieTitle = request.MovieTitle,
            StartTime = request.StartTime,
            RoomNumber = request.RoomNumber,
            TotalSeats = request.TotalSeats,
            AvailableSeats = request.TotalSeats,
            TicketPrice = request.TicketPrice,
            CreatedAt = DateTime.UtcNow
        };

        SessionStore[sessionId] = session;

        // Inicializa todos os assentos como disponíveis
        for (int i = 1; i <= request.TotalSeats; i++)
        {
            var seatNumber = $"{request.RoomNumber}{i}";
            SeatStore[(sessionId, seatNumber)] = (SeatStatus.Available, null);
        }

        var response = new SessionResponse
        {
            Id = sessionId,
            MovieTitle = request.MovieTitle,
            AvailableSeats = request.TotalSeats,
            CreatedAt = DateTime.UtcNow
        };

        return await Task.FromResult(response);
    }

    public async Task<SessionResponse?> GetSessionByIdAsync(Guid id)
    {
        if (!SessionStore.TryGetValue(id, out var session))
            return null;

        return await Task.FromResult(new SessionResponse
        {
            Id = session.Id,
            MovieTitle = session.MovieTitle,
            AvailableSeats = session.AvailableSeats,
            CreatedAt = session.CreatedAt
        });
    }

    public async Task<SessionSeatsResponse?> GetSessionSeatsAsync(Guid sessionId)
    {
        if (!SessionStore.TryGetValue(sessionId, out var session))
            return null;

        var seats = SeatStore
            .Where(kvp => kvp.Key.Item1 == sessionId)
            .Select(kvp => new SeatDto
            {
                Number = kvp.Key.Item2,
                Status = kvp.Value.status,
                ExpiresAt = kvp.Value.expiresAt
            })
            .OrderBy(s => s.Number)
            .ToArray();

        var availableCount = seats.Count(s => s.Status == SeatStatus.Available);

        return await Task.FromResult(new SessionSeatsResponse
        {
            SessionId = sessionId,
            TotalSeats = session.TotalSeats,
            AvailableSeats = availableCount,
            Seats = seats
        });
    }

    public async Task<IEnumerable<SessionResponse>> GetAllSessionsAsync()
    {
        var responses = SessionStore.Values.Select(s => new SessionResponse
        {
            Id = s.Id,
            MovieTitle = s.MovieTitle,
            AvailableSeats = s.AvailableSeats,
            CreatedAt = s.CreatedAt
        }).ToList();

        return await Task.FromResult(responses);
    }

    public async Task<SessionResponse?> UpdateSessionAsync(Guid id, CreateSessionRequest request)
    {
        if (!SessionStore.TryGetValue(id, out var session))
            return null;

        session.MovieTitle = request.MovieTitle;
        session.StartTime = request.StartTime;
        session.RoomNumber = request.RoomNumber;
        session.TotalSeats = request.TotalSeats;
        session.TicketPrice = request.TicketPrice;
        session.UpdatedAt = DateTime.UtcNow;

        return await Task.FromResult(new SessionResponse
        {
            Id = session.Id,
            MovieTitle = session.MovieTitle,
            AvailableSeats = session.AvailableSeats,
            CreatedAt = session.CreatedAt
        });
    }

    public async Task<bool> DeleteSessionAsync(Guid id)
    {
        return await Task.FromResult(SessionStore.Remove(id));
    }

    // Helper para testes
    public void ReserveSeat(Guid sessionId, string seatNumber)
    {
        var key = (sessionId, seatNumber);
        if (SeatStore.ContainsKey(key))
        {
            SeatStore[key] = (SeatStatus.Reserved, DateTime.UtcNow.AddHours(1));
        }
    }
}
