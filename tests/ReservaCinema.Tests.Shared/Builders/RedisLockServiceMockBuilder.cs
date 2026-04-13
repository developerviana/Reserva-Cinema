using Moq;
using StackExchange.Redis;

namespace ReservaCinema.Tests.Shared.Builders;

/// <summary>
/// Builder para construir mocks de Redis para testes do RedisLockService.
/// Simplifica a configuração de cenários de lock distribuído.
/// Referência: https://github.com/StackExchange/StackExchange.Redis
/// Nota: ScriptEvaluateAsync retorna RedisResult que é complexo mockar.
/// Alternativa: usar integração com Redis real ou simplificar o mock para o essencial.
/// </summary>
public class RedisLockServiceMockBuilder
{
    private bool _lockAcquired = true;
    private bool _lockExists = true;
    private string _storedToken = "";

    public RedisLockServiceMockBuilder WithAcquireLock(bool acquired)
    {
        _lockAcquired = acquired;
        return this;
    }

    public RedisLockServiceMockBuilder WithStoredToken(string token)
    {
        _lockExists = !string.IsNullOrEmpty(token);
        _storedToken = token;
        return this;
    }

    public RedisLockServiceMockBuilder WithNoLock()
    {
        _lockExists = false;
        _storedToken = "";
        return this;
    }

    /// <summary>
    /// Constrói um mock completo de IConnectionMultiplexer para RedisLockService.
    /// NOTA IMPORTANTE: RedisResult não pode ser instanciado diretamente em testes.
    /// Para casos que precisam testar ReleaseLockAsync, use testes de integração com Redis real.
    /// Este builder funciona para AcquireLockAsync e IsLockOwnedAsync.
    /// </summary>
    public Mock<IConnectionMultiplexer> Build()
    {
        var mockDb = new Mock<IDatabase>(MockBehavior.Strict); // Strict para detectar chamadas inesperadas
        var mockConnection = new Mock<IConnectionMultiplexer>();

        // StringSetAsync para AcquireLockAsync (SET NX EX)
        mockDb
            .Setup(db => db.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan?>(),
                When.NotExists))
            .ReturnsAsync(_lockAcquired);

        // ScriptEvaluateAsync para ReleaseLockAsync
        // RedisResult não pode ser criado em testes. 
        // Deixamos com behavior default (null) - testes de integração devem validar release.
        mockDb
            .Setup(db => db.ScriptEvaluateAsync(
                It.IsAny<string>(),
                It.IsAny<RedisKey[]>(),
                It.IsAny<RedisValue[]>(),
                It.IsAny<CommandFlags>()))
            .ThrowsAsync(new NotImplementedException("Use RedisLockServiceIntegrationTests para testar ReleaseLockAsync"));

        // StringGetAsync para IsLockOwnedAsync (GET)
        mockDb
            .Setup(db => db.StringGetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisValue)(_lockExists ? _storedToken : RedisValue.Null));

        mockConnection
            .Setup(conn => conn.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(mockDb.Object);

        return mockConnection;
    }

    /// <summary>
    /// Cria um builder com cenário de sucesso padrão.
    /// </summary>
    public static RedisLockServiceMockBuilder CreateSuccessScenario()
    {
        var builder = new RedisLockServiceMockBuilder();
        builder.WithAcquireLock(true);
        builder.WithStoredToken(Guid.NewGuid().ToString());
        return builder;
    }

    /// <summary>
    /// Cria um builder com cenário de conflito (lock já existe).
    /// </summary>
    public static RedisLockServiceMockBuilder CreateConflictScenario()
    {
        return new RedisLockServiceMockBuilder()
            .WithAcquireLock(false)
            .WithStoredToken(Guid.NewGuid().ToString());
    }

    /// <summary>
    /// Cria um builder com cenário de lock não encontrado.
    /// </summary>
    public static RedisLockServiceMockBuilder CreateNoLockScenario()
    {
        return new RedisLockServiceMockBuilder()
            .WithNoLock();
    }
}
