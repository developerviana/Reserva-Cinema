using StackExchange.Redis;

namespace ReservaCinema.Infrastructure.Distributed;

/// <summary>
/// Abstrai a conexão com Redis.
/// Permite desacoplar RedisLockService da implementação específica do StackExchange.Redis.
/// </summary>
public interface IRedisConnectionProvider
{
    /// <summary>
    /// Obtém a conexão multiplexada do Redis.
    /// </summary>
    IConnectionMultiplexer ConnectionMultiplexer { get; }

    /// <summary>
    /// Obtém uma instância do database Redis.
    /// </summary>
    IDatabase GetDatabase();
}
