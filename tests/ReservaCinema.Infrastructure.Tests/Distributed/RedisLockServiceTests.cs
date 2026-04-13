using FluentAssertions;
using ReservaCinema.Infrastructure.Distributed;
using ReservaCinema.Tests.Shared.Builders;

namespace ReservaCinema.Infrastructure.Tests.Distributed;

/// <summary>
/// Testes unitários para RedisLockService - Parte 1.
/// Validam AcquireLockAsync e IsLockOwnedAsync com mocks.
/// ReleaseLockAsync será testado em integração com Redis real.
/// 
/// Razão: RedisResult não pode ser instanciado em testes unitários,
/// então usamos integração para validar o script Lua.
/// Referência TDD: Red → Green → Refactor
/// </summary>
public class RedisLockServiceTests
{
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
        var service = new RedisLockService(mockConnection.Object);

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
        var service = new RedisLockService(mockConnection.Object);

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
        var service = new RedisLockService(mockConnection.Object);

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
        var service = new RedisLockService(mockConnection.Object);

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
        var service = new RedisLockService(mockConnection.Object);

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
        var service = new RedisLockService(mockConnection.Object);

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
        var service = new RedisLockService(mockConnection.Object);

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
        var service = new RedisLockService(mockConnection.Object);

        // Act & Assert
        await FluentActions.Invoking(() => service.IsLockOwnedAsync(lockKey, invalidToken))
            .Should()
            .ThrowAsync<ArgumentException>();
    }

    #endregion

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullConnectionMultiplexer_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        FluentActions.Invoking(() => new RedisLockService(null!))
            .Should()
            .Throw<ArgumentNullException>();
    }

    #endregion
}
