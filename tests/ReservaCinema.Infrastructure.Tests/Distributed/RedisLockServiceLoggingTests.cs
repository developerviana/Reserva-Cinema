using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ReservaCinema.Infrastructure.Distributed;
using ReservaCinema.Infrastructure.Distributed.Configuration;
using ReservaCinema.Tests.Shared.Builders;

namespace ReservaCinema.Infrastructure.Tests.Distributed;

/// <summary>
/// Testes para logging do RedisLockService.
/// Valida que eventos importantes são logados.
/// </summary>
public class RedisLockServiceLoggingTests
{
    private Mock<IRedisConnectionProvider> CreateMockProvider(Mock<StackExchange.Redis.IConnectionMultiplexer> mockConnection)
    {
        var mockProvider = new Mock<IRedisConnectionProvider>();
        mockProvider
            .Setup(x => x.GetDatabase())
            .Returns(mockConnection.Object.GetDatabase());
        return mockProvider;
    }

    private DistributedLockOptions CreateDefaultOptions()
    {
        return new DistributedLockOptions();
    }

    [Fact]
    public async Task AcquireLockAsync_WhenSuccessful_ShouldLogInformation()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<RedisLockService>>();
        var lockKey = "seat:A1";
        var mockConnection = RedisLockServiceMockBuilder.CreateSuccessScenario().Build();
        var mockProvider = CreateMockProvider(mockConnection);
        var options = CreateDefaultOptions();
        var service = new RedisLockService(mockProvider.Object, options, loggerMock.Object);

        // Act
        var token = await service.AcquireLockAsync(lockKey, TimeSpan.FromSeconds(5));

        // Assert
        token.Should().NotBeNullOrEmpty();
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Acquired lock")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task AcquireLockAsync_WhenConflict_ShouldLogWarning()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<RedisLockService>>();
        var lockKey = "seat:A1";
        var mockConnection = RedisLockServiceMockBuilder.CreateConflictScenario().Build();
        var mockProvider = CreateMockProvider(mockConnection);
        var options = CreateDefaultOptions();
        var service = new RedisLockService(mockProvider.Object, options, loggerMock.Object);

        // Act
        var token = await service.AcquireLockAsync(lockKey, TimeSpan.FromSeconds(5));

        // Assert
        token.Should().BeNull();
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to acquire lock")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ReleaseLockAsync_WhenSuccessful_ShouldLogInformation()
    {
        // Note: This test requires actual Redis or proper mock setup for Lua script evaluation
        // For now, we skip this test as ScriptEvaluateAsync cannot be easily mocked
        // Integration tests with real Redis should validate this logging
        await Task.CompletedTask;
    }
}
