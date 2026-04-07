using ReservaCinema.Application.DTOs.Sessions;

namespace ReservaCinema.Application.Services.Interfaces;

/// <summary>
/// Interface para serviço de gerenciamento de sessões de cinema.
/// </summary>
public interface ISessionService
{
    /// <summary>
    /// Cria uma nova sessão de cinema.
    /// </summary>
    /// <param name="request">Dados da sessão a ser criada.</param>
    /// <returns>Retorna a sessão criada.</returns>
    Task<SessionResponse> CreateSessionAsync(CreateSessionRequest request);

    /// <summary>
    /// Obtém uma sessão pelo ID.
    /// </summary>
    /// <param name="id">ID da sessão.</param>
    /// <returns>Retorna a sessão ou null se não encontrada.</returns>
    Task<SessionResponse?> GetSessionByIdAsync(Guid id);

    /// <summary>
    /// Lista todas as sessões de cinema.
    /// </summary>
    /// <returns>Lista de sessões.</returns>
    Task<IEnumerable<SessionResponse>> GetAllSessionsAsync();

    /// <summary>
    /// Obtém a disponibilidade em tempo real dos assentos de uma sessão.
    /// </summary>
    /// <param name="sessionId">ID da sessão.</param>
    /// <returns>Retorna informações de assentos e disponibilidade ou null se sessão não encontrada.</returns>
    Task<SessionSeatsResponse?> GetSessionSeatsAsync(Guid sessionId);

    /// <summary>
    /// Atualiza uma sessão existente.
    /// </summary>
    /// <param name="id">ID da sessão.</param>
    /// <param name="request">Dados atualizados.</param>
    /// <returns>Retorna a sessão atualizada ou null se não encontrada.</returns>
    Task<SessionResponse?> UpdateSessionAsync(Guid id, CreateSessionRequest request);

    /// <summary>
    /// Deleta uma sessão.
    /// </summary>
    /// <param name="id">ID da sessão a deletar.</param>
    /// <returns>True se deletada com sucesso, false caso contrário.</returns>
    Task<bool> DeleteSessionAsync(Guid id);
}
