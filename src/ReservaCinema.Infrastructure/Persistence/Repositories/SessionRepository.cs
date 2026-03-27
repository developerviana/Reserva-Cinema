using Microsoft.EntityFrameworkCore;
using ReservaCinema.Application.Persistence.Repositories;
using ReservaCinema.Domain.Entities;

namespace ReservaCinema.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação do repositório de sessões.
/// </summary>
public class SessionRepository : ISessionRepository
{
    private readonly ReservaCinemaDbContext _context;

    public SessionRepository(ReservaCinemaDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Adiciona uma nova sessão ao banco de dados.
    /// </summary>
    public async Task<Session> AddAsync(Session session)
    {
        _context.Sessions.Add(session);
        await _context.SaveChangesAsync();
        return session;
    }

    /// <summary>
    /// Obtém uma sessão pelo ID.
    /// </summary>
    public async Task<Session?> GetByIdAsync(Guid id)
    {
        return await _context.Sessions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    /// <summary>
    /// Lista todas as sessões ativas.
    /// </summary>
    public async Task<IEnumerable<Session>> GetAllAsync()
    {
        return await _context.Sessions
            .Where(s => s.IsActive)
            .AsNoTracking()
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    /// <summary>
    /// Atualiza uma sessão.
    /// </summary>
    public async Task<Session> UpdateAsync(Session session)
    {
        session.UpdatedAt = DateTime.UtcNow;
        _context.Sessions.Update(session);
        await _context.SaveChangesAsync();
        return session;
    }

    /// <summary>
    /// Deleta uma sessão (soft delete).
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var session = await _context.Sessions.FindAsync(id);
        if (session == null)
            return false;

        session.IsActive = false;
        session.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Verifica se uma sessão existe.
    /// </summary>
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Sessions.AnyAsync(s => s.Id == id && s.IsActive);
    }
}
