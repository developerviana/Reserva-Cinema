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
