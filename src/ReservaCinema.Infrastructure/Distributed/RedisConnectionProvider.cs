using StackExchange.Redis;

namespace ReservaCinema.Infrastructure.Distributed;

/// <summary>
/// Implementação padrão de IRedisConnectionProvider.
/// Encapsula a conexão multiplexada do Redis.
/// </summary>
public class RedisConnectionProvider : IRedisConnectionProvider
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public IConnectionMultiplexer ConnectionMultiplexer =>
        _connectionMultiplexer ?? throw new InvalidOperationException("Connection not initialized.");

    public RedisConnectionProvider(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
    }

    public IDatabase GetDatabase()
    {
        return _connectionMultiplexer.GetDatabase();
    }
}
