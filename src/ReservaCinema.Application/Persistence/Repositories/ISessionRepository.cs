using ReservaCinema.Domain.Entities;

namespace ReservaCinema.Application.Persistence.Repositories;

/// <summary>
/// Interface do repositório para gerenciar sessões.
/// </summary>
public interface ISessionRepository
{
    /// <summary>
    /// Adiciona uma nova sessão ao banco de dados.
    /// </summary>
    Task<Session> AddAsync(Session session);

    /// <summary>
    /// Obtém uma sessão pelo ID.
    /// </summary>
    Task<Session?> GetByIdAsync(Guid id);

    /// <summary>
    /// Obtém todas as sessões.
    /// </summary>
    Task<IEnumerable<Session>> GetAllAsync();

    /// <summary>
    /// Atualiza uma sessão existente.
    /// </summary>
    Task<Session> UpdateAsync(Session session);

    /// <summary>
    /// Deleta uma sessão pelo ID.
    /// </summary>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Verifica se uma sessão existe.
    /// </summary>
    Task<bool> ExistsAsync(Guid id);
}
