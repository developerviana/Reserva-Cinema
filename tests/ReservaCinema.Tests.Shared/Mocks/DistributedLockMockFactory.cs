using Moq;
using StackExchange.Redis;

namespace ReservaCinema.Tests.Shared.Mocks;

/// <summary>
/// Factory para criar mocks de serviços de lock distribuído.
/// </summary>
public static class DistributedLockMockFactory
{
    /// <summary>
    /// Cria um mock do IConnectionMultiplexer para Redis.
    /// </summary>
    public static Mock<IConnectionMultiplexer> CreateConnectionMultiplexerMock()
    {
        return new Mock<IConnectionMultiplexer>();
    }

    /// <summary>
    /// Cria um mock do IDatabase (Redis Database).
    /// </summary>
    public static Mock<IDatabase> CreateDatabaseMock()
    {
        return new Mock<IDatabase>();
    }

    /// <summary>
    /// Cria um IConnectionMultiplexer mock pré-configurado para sucesso na aquisição de lock.
    /// </summary>
    public static Mock<IConnectionMultiplexer> CreateConnectionMultiplexerMockWithSuccessfulLockAcquisition()
    {
        var mockDb = new Mock<IDatabase>();
        var mockConnection = new Mock<IConnectionMultiplexer>();

        mockDb
            .Setup(db => db.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(), When.NotExists, CommandFlags.None))
            .ReturnsAsync(true);

        mockConnection
            .Setup(conn => conn.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(mockDb.Object);

        return mockConnection;
    }

    /// <summary>
    /// Cria um IConnectionMultiplexer mock pré-configurado para falha na aquisição de lock.
    /// </summary>
    public static Mock<IConnectionMultiplexer> CreateConnectionMultiplexerMockWithFailedLockAcquisition()
    {
        var mockDb = new Mock<IDatabase>();
        var mockConnection = new Mock<IConnectionMultiplexer>();

        mockDb
            .Setup(db => db.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(), When.NotExists, CommandFlags.None))
            .ReturnsAsync(false);

        mockConnection
            .Setup(conn => conn.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(mockDb.Object);

        return mockConnection;
    }
}
