namespace ReservaCinema.Infrastructure.Distributed.Configuration;

/// <summary>
/// Opções de configuração para distributed lock.
/// Permite customização sem alterar código.
/// </summary>
public class DistributedLockOptions
{
    /// <summary>
    /// Tempo de expiração do lock em segundos.
    /// Padrão: 5 (deadlock prevention)
    /// </summary>
    public int LockExpirationSeconds { get; set; } = 5;

    /// <summary>
    /// Máximo de tentativas de aquisição de lock com retry.
    /// Padrão: 3
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Delay entre tentativas de retry em milissegundos.
    /// Padrão: 100
    /// </summary>
    public int RetryDelayMilliseconds { get; set; } = 100;
}
