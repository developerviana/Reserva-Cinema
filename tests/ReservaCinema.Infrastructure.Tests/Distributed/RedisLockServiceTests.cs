using FluentAssertions;
using Moq;
using ReservaCinema.Infrastructure.Distributed;
using ReservaCinema.Infrastructure.Distributed.Configuration;
using ReservaCinema.Tests.Shared.Builders;
using StackExchange.Redis;

namespace ReservaCinema.Infrastructure.Tests.Distributed;

/// <summary>
/// Testes unitários para RedisLockService.
/// Validam AcquireLockAsync e IsLockOwnedAsync com mocks.
/// ReleaseLockAsync será testado em integração com Redis real (Lua script).
/// 
/// TDD: Red → Green → Refactor
/// </summary>
public class RedisLockServiceTests
{
    private DistributedLockOptions CreateDefaultOptions()
    {
        return new DistributedLockOptions
        {
            LockExpirationSeconds = 5,
            MaxRetryAttempts = 3,
            RetryDelayMilliseconds = 100
        };
    }

    private Mock<IRedisConnectionProvider> CreateMockProvider(Mock<IConnectionMultiplexer> mockConnection)
    {
        var mockProvider = new Mock<IRedisConnectionProvider>();
        mockProvider
            .Setup(x => x.GetDatabase())
            .Returns(mockConnection.Object.GetDatabase());
        mockProvider
            .Setup(x => x.ConnectionMultiplexer)
            .Returns(mockConnection.Object);
        return mockProvider;
    }

    #region AcquireLockAsync Tests

    [Fact]
    public async Task AcquireLockAsync_WithValidParameters_ShouldReturnToken()
    {
        // Arrange
        var lockKey = "seat:123";
        var expiration = TimeSpan.FromSeconds(5);
        var mockConnection = RedisLockServiceMockBuilder
            .CreateSuccessScenario()
            .Build();
        var mockProvider = CreateMockProvider(mockConnection);
        var options = CreateDefaultOptions();
        var service = new RedisLockService(mockProvider.Object, options);

        // Act
        var token = await service.AcquireLockAsync(lockKey, expiration);

        // Assert
        token.Should().NotBeNullOrWhiteSpace();
        token.Should().MatchRegex(@"^[a-f0-9\-]{36}$"); // Guid format
    }

    [Fact]
    public async Task AcquireLockAsync_WhenLockAlreadyExists_ShouldReturnNull()
    {
        // Arrange
        var lockKey = "seat:123";
        var expiration = TimeSpan.FromSeconds(5);
        var mockConnection = RedisLockServiceMockBuilder
            .CreateConflictScenario()
            .Build();
        var mockProvider = CreateMockProvider(mockConnection);
        var options = CreateDefaultOptions();
        var service = new RedisLockService(mockProvider.Object, options);

        // Act
        var token = await service.AcquireLockAsync(lockKey, expiration);

        // Assert
        token.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task AcquireLockAsync_WithInvalidLockKey_ShouldThrowArgumentException(string invalidKey)
    {
        // Arrange
        var expiration = TimeSpan.FromSeconds(5);
        var mockConnection = RedisLockServiceMockBuilder
            .CreateSuccessScenario()
            .Build();
        var mockProvider = CreateMockProvider(mockConnection);
        var options = CreateDefaultOptions();
        var service = new RedisLockService(mockProvider.Object, options);

        // Act & Assert
        await FluentActions.Invoking(() => service.AcquireLockAsync(invalidKey, expiration))
            .Should()
            .ThrowAsync<ArgumentException>();
    }

    #endregion

    #region IsLockOwnedAsync Tests

    [Fact]
    public async Task IsLockOwnedAsync_WithValidToken_ShouldReturnTrue()
    {
        // Arrange
        var lockKey = "seat:123";
        var lockToken = Guid.NewGuid().ToString();
        var mockConnection = new RedisLockServiceMockBuilder()
            .WithStoredToken(lockToken)
            .Build();
        var mockProvider = CreateMockProvider(mockConnection);
        var options = CreateDefaultOptions();
        var service = new RedisLockService(mockProvider.Object, options);

        // Act
        var isOwned = await service.IsLockOwnedAsync(lockKey, lockToken);

        // Assert
        isOwned.Should().BeTrue();
    }

    [Fact]
    public async Task IsLockOwnedAsync_WithInvalidToken_ShouldReturnFalse()
    {
        // Arrange
        var lockKey = "seat:123";
        var validToken = Guid.NewGuid().ToString();
        var invalidToken = Guid.NewGuid().ToString();
        var mockConnection = new RedisLockServiceMockBuilder()
            .WithStoredToken(validToken)
            .Build();
        var mockProvider = CreateMockProvider(mockConnection);
        var options = CreateDefaultOptions();
        var service = new RedisLockService(mockProvider.Object, options);

        // Act
        var isOwned = await service.IsLockOwnedAsync(lockKey, invalidToken);

        // Assert
        isOwned.Should().BeFalse();
    }

    [Fact]
    public async Task IsLockOwnedAsync_WhenLockExpired_ShouldReturnFalse()
    {
        // Arrange
        var lockKey = "seat:123";
        var lockToken = Guid.NewGuid().ToString();
        var mockConnection = RedisLockServiceMockBuilder
            .CreateNoLockScenario()
            .Build();
        var mockProvider = CreateMockProvider(mockConnection);
        var options = CreateDefaultOptions();
        var service = new RedisLockService(mockProvider.Object, options);

        // Act
        var isOwned = await service.IsLockOwnedAsync(lockKey, lockToken);

        // Assert
        isOwned.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task IsLockOwnedAsync_WithInvalidLockKey_ShouldThrowArgumentException(string invalidKey)
    {
        // Arrange
        var lockToken = Guid.NewGuid().ToString();
        var mockConnection = RedisLockServiceMockBuilder
            .CreateSuccessScenario()
            .Build();
        var mockProvider = CreateMockProvider(mockConnection);
        var options = CreateDefaultOptions();
        var service = new RedisLockService(mockProvider.Object, options);

        // Act & Assert
        await FluentActions.Invoking(() => service.IsLockOwnedAsync(invalidKey, lockToken))
            .Should()
            .ThrowAsync<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task IsLockOwnedAsync_WithInvalidToken_ShouldThrowArgumentException(string invalidToken)
    {
        // Arrange
        var lockKey = "seat:123";
        var mockConnection = RedisLockServiceMockBuilder
            .CreateSuccessScenario()
            .Build();
        var mockProvider = CreateMockProvider(mockConnection);
        var options = CreateDefaultOptions();
        var service = new RedisLockService(mockProvider.Object, options);

        // Act & Assert
        await FluentActions.Invoking(() => service.IsLockOwnedAsync(lockKey, invalidToken))
            .Should()
            .ThrowAsync<ArgumentException>();
    }

    #endregion

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullConnectionProvider_ShouldThrowArgumentNullException()
    {
        // Arrange
        var options = CreateDefaultOptions();

        // Act & Assert
        FluentActions.Invoking(() => new RedisLockService(null!, options))
            .Should()
            .Throw<ArgumentNullException>()
            .WithParameterName("connectionProvider");
    }

    [Fact]
    public void Constructor_WithNullOptions_ShouldThrowArgumentNullException()
    {
        // Arrange
        var mockConnection = RedisLockServiceMockBuilder
            .CreateSuccessScenario()
            .Build();
        var mockProvider = CreateMockProvider(mockConnection);

        // Act & Assert
        FluentActions.Invoking(() => new RedisLockService(mockProvider.Object, null!))
            .Should()
            .Throw<ArgumentNullException>()
            .WithParameterName("options");
    }

    #endregion
}
