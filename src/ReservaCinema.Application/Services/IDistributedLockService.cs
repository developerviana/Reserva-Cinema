namespace ReservaCinema.Application.Services;

/// <summary>
/// Interface para serviço de lock distribuído.
/// Garante exclusão mútua entre múltiplas requisições.
/// </summary>
public interface IDistributedLockService
{
    /// <summary>
    /// Adquire um lock de forma atômica.
    /// </summary>
    /// <returns>Token do lock se adquirido, null se já existe</returns>
    Task<string?> AcquireLockAsync(string lockKey, TimeSpan expiration);

    /// <summary>
    /// Libera um lock de forma segura.
    /// </summary>
    /// <returns>True se liberado com sucesso, False se não pertence ao token</returns>
    Task<bool> ReleaseLockAsync(string lockKey, string lockToken);

    /// <summary>
    /// Verifica se o lock pertence ao token.
    /// </summary>
    Task<bool> IsLockOwnedAsync(string lockKey, string lockToken);
}
