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
/// Testes unitários para ReservationService.
/// Regras de negócio testadas em isolamento com dependências mockadas.
/// Atualizado: validação movida para CreateReservationRequestValidator (mudança arquitetural documentada).
/// Atualizado: preço calculado a partir de Session.TicketPrice, não hardcoded (regra de negócio corrigida).
/// </summary>
public class ReservationServiceTests
{
    private readonly Mock<ISessionRepository> _mockSessionRepository;
    private readonly Mock<IReservationRepository> _mockReservationRepository;
    private readonly Mock<IDistributedLockService> _mockLockService;
    private readonly ReservationService _service;

    public ReservationServiceTests()
    {
        _mockSessionRepository = new Mock<ISessionRepository>();
        _mockReservationRepository = new Mock<IReservationRepository>();
        _mockLockService = new Mock<IDistributedLockService>();

        _mockLockService
            .Setup(x => x.AcquireLockAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .ReturnsAsync(Guid.NewGuid().ToString());
        _mockLockService
            .Setup(x => x.ReleaseLockAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        _mockReservationRepository
            .Setup(x => x.AddAsync(It.IsAny<Reservation>()))
            .ReturnsAsync((Reservation r) => r);

        _service = new ReservationService(
            _mockSessionRepository.Object,
            _mockReservationRepository.Object,
            _mockLockService.Object);
    }

    [Fact]
    public async Task CreateReservation_QuandoSessaoNaoEncontrada_DeveLancarKeyNotFoundException()
    {
        _mockSessionRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Session?)null);

        var request = new CreateReservationRequest
        {
            SessionId = Guid.NewGuid(),
            UserId = "user-1",
            SeatNumbers = ["A1"]
        };

        await FluentActions.Invoking(() => _service.CreateReservationAsync(request))
            .Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task CreateReservation_QuandoAssentosInsuficientes_DeveLancarConflictException()
    {
        var session = new SessionBuilder().WithAvailableSeats(1).Build();
        _mockSessionRepository.Setup(x => x.GetByIdAsync(session.Id)).ReturnsAsync(session);

        var request = new CreateReservationRequest
        {
            SessionId = session.Id,
            UserId = "user-1",
            SeatNumbers = ["A1", "A2"]
        };

        await FluentActions.Invoking(() => _service.CreateReservationAsync(request))
            .Should().ThrowAsync<ConflictException>();
    }

    [Theory]
    [InlineData(1, 30.00)]
    [InlineData(2, 60.00)]
    [InlineData(5, 150.00)]
    public async Task CreateReservation_DeveCalcularPrecoComBaseNaSessao(int qtdAssentos, decimal precoEsperado)
    {
        var session = new SessionBuilder().WithTicketPrice(30.00m).WithAvailableSeats(10).Build();
        _mockSessionRepository.Setup(x => x.GetByIdAsync(session.Id)).ReturnsAsync(session);

        var seats = Enumerable.Range(1, qtdAssentos).Select(i => $"A{i}").ToArray();
        var request = new CreateReservationRequest
        {
            SessionId = session.Id,
            UserId = "user-1",
            SeatNumbers = seats
        };

        var response = await _service.CreateReservationAsync(request);

        response.TotalAmount.Should().Be(precoEsperado);
    }

    [Fact]
    public async Task CreateReservation_ChaveDeLockDeveConterSessionId()
    {
        var sessionId = Guid.NewGuid();
        var session = new SessionBuilder().WithId(sessionId).WithAvailableSeats(10).Build();
        _mockSessionRepository.Setup(x => x.GetByIdAsync(sessionId)).ReturnsAsync(session);

        var request = new CreateReservationRequest
        {
            SessionId = sessionId,
            UserId = "user-1",
            SeatNumbers = ["A1"]
        };

        await _service.CreateReservationAsync(request);

        _mockLockService.Verify(
            x => x.AcquireLockAsync(
                It.Is<string>(k => k.Contains(sessionId.ToString())),
                It.IsAny<TimeSpan>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateReservation_DevePersistrReserva()
    {
        var session = new SessionBuilder().WithAvailableSeats(10).Build();
        _mockSessionRepository.Setup(x => x.GetByIdAsync(session.Id)).ReturnsAsync(session);

        var request = new CreateReservationRequest
        {
            SessionId = session.Id,
            UserId = "user-1",
            SeatNumbers = ["A1"]
        };

        await _service.CreateReservationAsync(request);

        _mockReservationRepository.Verify(x => x.AddAsync(It.IsAny<Reservation>()), Times.Once);
    }

    [Fact]
    public async Task CreateReservation_DeveRetornarStatusPendente()
    {
        var session = new SessionBuilder().WithAvailableSeats(10).Build();
        _mockSessionRepository.Setup(x => x.GetByIdAsync(session.Id)).ReturnsAsync(session);

        var request = new CreateReservationRequest
        {
            SessionId = session.Id,
            UserId = "user-1",
            SeatNumbers = ["A1", "A2"]
        };

        var response = await _service.CreateReservationAsync(request);

        response.Status.Should().Be("pending");
        response.ReservationId.Should().NotBeNullOrEmpty();
        response.Seats.Should().BeEquivalentTo(request.SeatNumbers);
        response.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddHours(1), TimeSpan.FromSeconds(5));
    }
}
