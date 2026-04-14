using FluentAssertions;
using Moq;
using ReservaCinema.Infrastructure.Distributed;
using StackExchange.Redis;

namespace ReservaCinema.Infrastructure.Tests.Distributed;

/// <summary>
/// Testes para RedisConnectionProvider.
/// Valida que a abstração de conexão Redis é corretamente implementada.
/// </summary>
public class RedisConnectionProviderTests
{
    [Fact]
    public void Constructor_WhenConnectionMultiplexerIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        FluentActions.Invoking(() => new RedisConnectionProvider(null!))
            .Should()
            .Throw<ArgumentNullException>()
            .WithParameterName("connectionMultiplexer");
    }

    [Fact]
    public void GetDatabase_WhenCalled_ShouldReturnDatabaseFromMultiplexer()
    {
        // Arrange
        var mockDb = new Mock<IDatabase>();
        var mockConnection = new Mock<IConnectionMultiplexer>();
        mockConnection
            .Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(mockDb.Object);

        var provider = new RedisConnectionProvider(mockConnection.Object);

        // Act
        var result = provider.GetDatabase();

        // Assert
        result.Should().Be(mockDb.Object);
        mockConnection.Verify(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public void ConnectionMultiplexer_Property_ShouldReturnUnderlyingConnection()
    {
        // Arrange
        var mockConnection = new Mock<IConnectionMultiplexer>();
        var provider = new RedisConnectionProvider(mockConnection.Object);

        // Act
        var result = provider.ConnectionMultiplexer;

        // Assert
        result.Should().Be(mockConnection.Object);
    }
}
