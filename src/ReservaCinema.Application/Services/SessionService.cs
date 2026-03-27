using ReservaCinema.Application.DTOs.Sessions;
using ReservaCinema.Application.Services.Interfaces;

namespace ReservaCinema.Application.Services;

/// <summary>
/// Serviço para gerenciamento de sessões de cinema.
/// </summary>
public class SessionService : ISessionService
{
    // TODO: Injetar repositório aqui para persistência em banco de dados
    private readonly List<SessionResponse> _sessions = new();

    /// <summary>
    /// Cria uma nova sessão de cinema.
    /// </summary>
    public async Task<SessionResponse> CreateSessionAsync(CreateSessionRequest request)
    {
        var session = new SessionResponse
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

        _sessions.Add(session);
        return await Task.FromResult(session);
    }

    /// <summary>
    /// Obtém uma sessão pelo ID.
    /// </summary>
    public async Task<SessionResponse?> GetSessionByIdAsync(Guid id)
    {
        var session = _sessions.FirstOrDefault(s => s.Id == id);
        return await Task.FromResult(session);
    }

    /// <summary>
    /// Lista todas as sessões de cinema.
    /// </summary>
    public async Task<IEnumerable<SessionResponse>> GetAllSessionsAsync()
    {
        return await Task.FromResult(_sessions.AsEnumerable());
    }

    /// <summary>
    /// Atualiza uma sessão existente.
    /// </summary>
    public async Task<SessionResponse?> UpdateSessionAsync(Guid id, CreateSessionRequest request)
    {
        var session = _sessions.FirstOrDefault(s => s.Id == id);
        if (session == null)
            return await Task.FromResult<SessionResponse?>(null);

        session.MovieTitle = request.MovieTitle;
        session.StartTime = request.StartTime;
        session.RoomNumber = request.RoomNumber;
        session.TotalSeats = request.TotalSeats;
        session.TicketPrice = request.TicketPrice;

        return await Task.FromResult(session);
    }

    /// <summary>
    /// Deleta uma sessão.
    /// </summary>
    public async Task<bool> DeleteSessionAsync(Guid id)
    {
        var session = _sessions.FirstOrDefault(s => s.Id == id);
        if (session == null)
            return await Task.FromResult(false);

        _sessions.Remove(session);
        return await Task.FromResult(true);
    }
}
