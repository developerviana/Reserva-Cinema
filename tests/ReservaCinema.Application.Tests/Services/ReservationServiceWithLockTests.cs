using FluentAssertions;
using Moq;
using ReservaCinema.Application.DTOs.Reservations;
using ReservaCinema.Application.Persistence.Repositories;
using ReservaCinema.Application.Services;
using ReservaCinema.Domain.Entities;
using ReservaCinema.Domain.Exceptions;
using ReservaCinema.Tests.Shared.Builders;

namespace ReservaCinema.Application.Tests.Services;

/// <summary>
/// Testes de comportamento do lock distribuído no ReservationService.
/// Atualizado: construtor de ReservationService expandido com ISessionRepository e IReservationRepository
/// após implementação real de persistência (mudança arquitetural documentada).
/// </summary>
public class ReservationServiceWithLockTests
{
    private static ReservationService CreateService(
        Mock<IDistributedLockService> mockLockService,
        Session? session = null)
    {
        var mockSessionRepo = new Mock<ISessionRepository>();
        var sessionToReturn = session ?? new SessionBuilder().WithAvailableSeats(10).Build();
        mockSessionRepo
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(sessionToReturn);

        var mockReservationRepo = new Mock<IReservationRepository>();
        mockReservationRepo
            .Setup(x => x.AddAsync(It.IsAny<Reservation>()))
            .ReturnsAsync((Reservation r) => r);

        return new ReservationService(
            mockSessionRepo.Object,
            mockReservationRepo.Object,
            mockLockService.Object);
    }

    [Fact]
    public async Task CreateReservationAsync_ShouldAcquireLockBeforeProcessing()
    {
        var mockLockService = new Mock<IDistributedLockService>();
        mockLockService
            .Setup(x => x.AcquireLockAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .ReturnsAsync(Guid.NewGuid().ToString());

        var service = CreateService(mockLockService);
        var request = new CreateReservationRequest
        {
            SessionId = Guid.NewGuid(),
            UserId = "user-123",
            SeatNumbers = ["A1", "A2"]
        };

        var response = await service.CreateReservationAsync(request);

        response.Should().NotBeNull();
        mockLockService.Verify(
            x => x.AcquireLockAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()),
            Times.Once,
            "Lock deve ser adquirido antes de processar a reserva");
    }

    [Fact]
    public async Task CreateReservationAsync_ShouldReleaseLockAfterProcessing()
    {
        var lockToken = Guid.NewGuid().ToString();
        var mockLockService = new Mock<IDistributedLockService>();
        mockLockService
            .Setup(x => x.AcquireLockAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .ReturnsAsync(lockToken);
        mockLockService
            .Setup(x => x.ReleaseLockAsync(It.IsAny<string>(), lockToken))
            .ReturnsAsync(true);

        var service = CreateService(mockLockService);
        var request = new CreateReservationRequest
        {
            SessionId = Guid.NewGuid(),
            UserId = "user-123",
            SeatNumbers = ["A1"]
        };

        var response = await service.CreateReservationAsync(request);

        response.Should().NotBeNull();
        mockLockService.Verify(
            x => x.ReleaseLockAsync(It.IsAny<string>(), lockToken),
            Times.Once,
            "Lock deve ser liberado após processar a reserva");
    }

    [Fact]
    public async Task CreateReservationAsync_WhenLockAcquisitionFails_ShouldThrowConflictException()
    {
        var mockLockService = new Mock<IDistributedLockService>();
        mockLockService
            .Setup(x => x.AcquireLockAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .ReturnsAsync((string?)null);

        var service = CreateService(mockLockService);
        var request = new CreateReservationRequest
        {
            SessionId = Guid.NewGuid(),
            UserId = "user-123",
            SeatNumbers = ["A1"]
        };

        await FluentActions.Invoking(() => service.CreateReservationAsync(request))
            .Should()
            .ThrowAsync<ConflictException>()
            .WithMessage("*lock*");
    }
}
