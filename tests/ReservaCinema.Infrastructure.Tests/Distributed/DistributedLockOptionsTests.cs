using FluentAssertions;
using ReservaCinema.Infrastructure.Distributed.Configuration;

namespace ReservaCinema.Infrastructure.Tests.Distributed;

/// <summary>
/// Testes para DistributedLockOptions.
/// Valida configurações de lock distribuído.
/// </summary>
public class DistributedLockOptionsTests
{
    [Fact]
    public void DefaultConstructor_ShouldHaveValidDefaults()
    {
        // Act
        var options = new DistributedLockOptions();

        // Assert
        options.LockExpirationSeconds.Should().Be(5);
        options.MaxRetryAttempts.Should().Be(3);
        options.RetryDelayMilliseconds.Should().Be(100);
    }

    [Fact]
    public void LockExpirationSeconds_ShouldBeSettable()
    {
        // Arrange
        var options = new DistributedLockOptions();

        // Act
        options.LockExpirationSeconds = 10;

        // Assert
        options.LockExpirationSeconds.Should().Be(10);
    }

    [Fact]
    public void MaxRetryAttempts_ShouldBeSettable()
    {
        // Arrange
        var options = new DistributedLockOptions();

        // Act
        options.MaxRetryAttempts = 5;

        // Assert
        options.MaxRetryAttempts.Should().Be(5);
    }

    [Fact]
    public void RetryDelayMilliseconds_ShouldBeSettable()
    {
        // Arrange
        var options = new DistributedLockOptions();

        // Act
        options.RetryDelayMilliseconds = 200;

        // Assert
        options.RetryDelayMilliseconds.Should().Be(200);
    }
}
