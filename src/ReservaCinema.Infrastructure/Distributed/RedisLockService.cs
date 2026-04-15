using Microsoft.Extensions.Logging;
using ReservaCinema.Application.Services;
using ReservaCinema.Infrastructure.Distributed.Configuration;
using StackExchange.Redis;

namespace ReservaCinema.Infrastructure.Distributed;

/// <summary>
/// Implementa distributed lock usando Redis com padrão Redlock.
/// Garante operações atômicas para evitar race conditions em reservas concorrentes.
/// </summary>
public class RedisLockService : IDistributedLockService
{
    private readonly IRedisConnectionProvider _connectionProvider;
    private readonly DistributedLockOptions _options;
    private readonly ILogger<RedisLockService> _logger;

    /// <summary>
    /// Lua script para release seguro do lock.
    /// Verifica ownership antes de deletar, prevenindo que uma thread delete lock de outra.
    /// Retorna 1 se sucesso, 0 se lock não pertence ao token.
    /// </summary>
    private const string ReleaseLockScript = @"
        if redis.call('get', KEYS[1]) == ARGV[1] then
            return redis.call('del', KEYS[1])
        else
            return 0
        end
    ";

    public RedisLockService(
        IRedisConnectionProvider connectionProvider,
        DistributedLockOptions options,
        ILogger<RedisLockService> logger = null!)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger;
    }

    public async Task<string?> AcquireLockAsync(string lockKey, TimeSpan expiration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(lockKey);

        var token = Guid.NewGuid().ToString();
        var db = _connectionProvider.GetDatabase();

        // SET NX EX: operação atômica no Redis
        // NX = Set if Not eXists (garante que só uma thread consegue)
        // EX = Com expiração em segundos (previne deadlock se processo morrer)
        var acquired = await db.StringSetAsync(
            lockKey,
            token,
            expiration,
            When.NotExists
        );

        if (acquired)
        {
            _logger?.LogInformation("Acquired lock for key '{LockKey}' with token '{Token}'", lockKey, token);
            return token;
        }

        _logger?.LogWarning("Failed to acquire lock for key '{LockKey}'. Lock already exists or is not available", lockKey);
        return null;
    }

    /// <summary>
    /// Libera o lock de forma segura verificando ownership com Lua script.
    /// </summary>
    /// <param name="lockKey">Chave do lock</param>
    /// <param name="lockToken">Token (prova de ownership)</param>
    /// <returns>True se sucesso, false se lock não existe ou não pertence ao token</returns>
    public async Task<bool> ReleaseLockAsync(string lockKey, string lockToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(lockKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(lockToken);

        var db = _connectionProvider.GetDatabase();

        // Executa Lua script atomicamente
        // Script verifica se valor armazenado == token antes de deletar
        var result = await db.ScriptEvaluateAsync(
            ReleaseLockScript,
            new RedisKey[] { lockKey },
            new RedisValue[] { lockToken }
        );

        var released = result.IsNull == false && (long)result == 1;

        if (released)
        {
            _logger?.LogInformation("Released lock for key '{LockKey}'", lockKey);
        }
        else
        {
            _logger?.LogWarning("Failed to release lock for key '{LockKey}'. Lock does not exist or token is invalid", lockKey);
        }

        return released;
    }

    /// <summary>
    /// Verifica se lock ainda existe e pertence ao token.
    /// </summary>
    /// <param name="lockKey">Chave do lock</param>
    /// <param name="lockToken">Token esperado</param>
    /// <returns>True se lock existe e token é válido</returns>
    public async Task<bool> IsLockOwnedAsync(string lockKey, string lockToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(lockKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(lockToken);

        var db = _connectionProvider.GetDatabase();

        // Simples GET: verifica se valor armazenado é igual ao token
        var storedToken = await db.StringGetAsync(lockKey);

        return !storedToken.IsNull && storedToken.ToString() == lockToken;
    }
}
