using FluentAssertions;
using Moq;
using ReservaCinema.Application.DTOs.Reservations;
using ReservaCinema.Application.Services;
using ReservaCinema.Domain.Exceptions;
using ReservaCinema.Tests.Shared.Builders;

namespace ReservaCinema.Application.Tests.Services;

/// <summary>
/// Testes de integração para ReservationService com lock distribuído.
/// Valida o padrão de aquisição de lock durante criação de reservas.
/// TDD: Red → Green → Refactor
/// </summary>
public class ReservationServiceWithLockTests
{
    [Fact]
    public async Task CreateReservationAsync_ShouldAcquireLockBeforeProcessing()
    {
        // Arrange
        var mockLockService = new Mock<IDistributedLockService>();
        mockLockService
            .Setup(x => x.AcquireLockAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .ReturnsAsync(Guid.NewGuid().ToString()); // Sucesso na aquisição

        var service = new ReservationService(mockLockService.Object);
        var request = new CreateReservationRequest
        {
            SessionId = Guid.NewGuid(),
            UserId = "user-123",
            SeatNumbers = new[] { "A1", "A2" }
        };

        // Act
        var response = await service.CreateReservationAsync(request);

        // Assert
        response.Should().NotBeNull();
        mockLockService.Verify(
            x => x.AcquireLockAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()),
            Times.Once,
            "Lock deve ser adquirido antes de processar a reserva");
    }

    [Fact]
    public async Task CreateReservationAsync_ShouldReleaseLockAfterProcessing()
    {
        // Arrange
        var lockToken = Guid.NewGuid().ToString();
        var mockLockService = new Mock<IDistributedLockService>();
        mockLockService
            .Setup(x => x.AcquireLockAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .ReturnsAsync(lockToken);
        mockLockService
            .Setup(x => x.ReleaseLockAsync(It.IsAny<string>(), lockToken))
            .ReturnsAsync(true);

        var service = new ReservationService(mockLockService.Object);
        var request = new CreateReservationRequest
        {
            SessionId = Guid.NewGuid(),
            UserId = "user-123",
            SeatNumbers = new[] { "A1" }
        };

        // Act
        var response = await service.CreateReservationAsync(request);

        // Assert
        response.Should().NotBeNull();
        mockLockService.Verify(
            x => x.ReleaseLockAsync(It.IsAny<string>(), lockToken),
            Times.Once,
            "Lock deve ser liberado após processar a reserva");
    }

    [Fact]
    public async Task CreateReservationAsync_WhenLockAcquisitionFails_ShouldThrowException()
    {
        // Arrange
        var mockLockService = new Mock<IDistributedLockService>();
        mockLockService
            .Setup(x => x.AcquireLockAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .ReturnsAsync((string?)null); // Falha na aquisição

        var service = new ReservationService(mockLockService.Object);
        var request = new CreateReservationRequest
        {
            SessionId = Guid.NewGuid(),
            UserId = "user-123",
            SeatNumbers = new[] { "A1" }
        };

        // Act & Assert
        await FluentActions.Invoking(() => service.CreateReservationAsync(request))
            .Should()
            .ThrowAsync<ConflictException>()
            .WithMessage("*lock*");
    }
}
