using ReservaCinema.Application.DTOs.Sessions;
using ReservaCinema.Application.Persistence.Repositories;
using ReservaCinema.Application.Services.Interfaces;
using ReservaCinema.Domain.Entities;

namespace ReservaCinema.Application.Services;

/// <summary>
/// Serviço para gerenciamento de sessões de cinema.
/// </summary>
public class SessionService : ISessionService
{
    private readonly ISessionRepository _repository;

    public SessionService(ISessionRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Cria uma nova sessão de cinema.
    /// </summary>
    public async Task<SessionResponse> CreateSessionAsync(CreateSessionRequest request)
    {
        // Validações de entrada
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

        var session = new Session
        {
            Id = Guid.NewGuid(),
            MovieTitle = request.MovieTitle,
            StartTime = request.StartTime,
            RoomNumber = request.RoomNumber,
            TotalSeats = request.TotalSeats,
            AvailableSeats = request.TotalSeats,
            TicketPrice = request.TicketPrice,
            CreatedAt = DateTime.UtcNow
        };

        var createdSession = await _repository.AddAsync(session);

        return MapToResponse(createdSession);
    }

    /// <summary>
    /// Obtém uma sessão pelo ID.
    /// </summary>
    public async Task<SessionResponse?> GetSessionByIdAsync(Guid id)
    {
        var session = await _repository.GetByIdAsync(id);
        return session == null ? null : MapToResponse(session);
    }

    /// <summary>
    /// Lista todas as sessões de cinema.
    /// </summary>
    public async Task<IEnumerable<SessionResponse>> GetAllSessionsAsync()
    {
        var sessions = await _repository.GetAllAsync();
        return sessions.Select(MapToResponse).ToList();
    }

    /// <summary>
    /// Atualiza uma sessão existente.
    /// </summary>
    public async Task<SessionResponse?> UpdateSessionAsync(Guid id, CreateSessionRequest request)
    {
        var session = await _repository.GetByIdAsync(id);
        if (session == null)
            return null;

        session.MovieTitle = request.MovieTitle;
        session.StartTime = request.StartTime;
        session.RoomNumber = request.RoomNumber;
        session.TotalSeats = request.TotalSeats;
        session.TicketPrice = request.TicketPrice;
        session.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(session);

        return MapToResponse(session);
    }

    /// <summary>
    /// Obtém a disponibilidade em tempo real dos assentos de uma sessão.
    /// </summary>
    public async Task<SessionSeatsResponse?> GetSessionSeatsAsync(Guid sessionId)
    {
        var session = await _repository.GetByIdAsync(sessionId);
        if (session == null)
            return null;

        // Simula a geração de assentos padrão (no caso real, viria do banco de dados)
        var seats = GenerateSeats(session.RoomNumber, session.TotalSeats);

        var availableCount = seats.Count(s => s.Status == SeatStatus.Available);

        var response = new SessionSeatsResponse
        {
            SessionId = sessionId,
            TotalSeats = session.TotalSeats,
            AvailableSeats = availableCount,
            Seats = seats
        };

        return await Task.FromResult(response);
    }

    /// <summary>
    /// Gera a lista padrão de assentos para uma sessão.
    /// </summary>
    private static SeatDto[] GenerateSeats(string roomNumber, int totalSeats)
    {
        var seats = new SeatDto[totalSeats];
        for (int i = 1; i <= totalSeats; i++)
        {
            seats[i - 1] = new SeatDto
            {
                Number = $"{roomNumber}{i}",
                Status = SeatStatus.Available
            };
        }
        return seats;
    }

    /// <summary>
    /// Deleta uma sessão.
    /// </summary>
    public async Task<bool> DeleteSessionAsync(Guid id)
    {
        var session = await _repository.GetByIdAsync(id);
        if (session == null)
            return false;

        await _repository.DeleteAsync(id);
        return true;
    }

    private static SessionResponse MapToResponse(Session session)
    {
        return new SessionResponse
        {
            Id = session.Id,
            MovieTitle = session.MovieTitle,
            StartTime = session.StartTime,
            RoomNumber = session.RoomNumber,
            TotalSeats = session.TotalSeats,
            AvailableSeats = session.AvailableSeats,
            TicketPrice = session.TicketPrice,
            CreatedAt = session.CreatedAt
        };
    }
}
