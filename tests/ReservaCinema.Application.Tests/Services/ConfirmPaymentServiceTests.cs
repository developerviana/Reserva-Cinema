using FluentAssertions;
using Moq;
using ReservaCinema.Application.DTOs.Reservations;
using ReservaCinema.Application.Persistence.Repositories;
using ReservaCinema.Application.Services;
using ReservaCinema.Application.Services.Interfaces;
using ReservaCinema.Domain.Entities;
using ReservaCinema.Domain.Exceptions;
using ReservaCinema.Tests.Shared.Builders;

namespace ReservaCinema.Application.Tests.Services;

/// <summary>
/// Testes unitários para ReservationService.ConfirmPaymentAsync.
/// Regras testadas: reserva não encontrada, reserva expirada, confirmação bem-sucedida.
/// </summary>
public class ConfirmPaymentServiceTests
{
    private readonly Mock<ISessionRepository> _mockSessionRepository;
    private readonly Mock<IReservationRepository> _mockReservationRepository;
    private readonly Mock<IDistributedLockService> _mockLockService;
    private readonly ReservationService _service;

    public ConfirmPaymentServiceTests()
    {
        _mockSessionRepository = new Mock<ISessionRepository>();
        _mockReservationRepository = new Mock<IReservationRepository>();
        _mockLockService = new Mock<IDistributedLockService>();

        _mockReservationRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Reservation>()))
            .ReturnsAsync((Reservation r) => r);

        _service = new ReservationService(
            _mockSessionRepository.Object,
            _mockReservationRepository.Object,
            _mockLockService.Object);
    }

    [Fact]
    public async Task ConfirmPayment_QuandoReservaNaoEncontrada_DeveLancarKeyNotFoundException()
    {
        _mockReservationRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((Reservation?)null);

        var request = new ConfirmPaymentRequest { PaymentMethod = "credit_card", TransactionId = "tx-1" };

        await FluentActions.Invoking(() => _service.ConfirmPaymentAsync("reserva-inexistente", request))
            .Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task ConfirmPayment_QuandoReservaExpirada_DeveLancarReservationExpiredException()
    {
        var reservation = new ReservationBuilder().WithExpiresAt(DateTime.UtcNow.AddMinutes(-1)).Build();
        _mockReservationRepository
            .Setup(x => x.GetByIdAsync(reservation.Id))
            .ReturnsAsync(reservation);

        var request = new ConfirmPaymentRequest { PaymentMethod = "credit_card", TransactionId = "tx-1" };

        await FluentActions.Invoking(() => _service.ConfirmPaymentAsync(reservation.Id, request))
            .Should().ThrowAsync<ReservationExpiredException>();
    }

    [Fact]
    public async Task ConfirmPayment_QuandoValido_DeveRetornarStatusConfirmed()
    {
        var reservation = new ReservationBuilder().WithSeats("A1", "A2").Build();
        _mockReservationRepository
            .Setup(x => x.GetByIdAsync(reservation.Id))
            .ReturnsAsync(reservation);

        var request = new ConfirmPaymentRequest { PaymentMethod = "credit_card", TransactionId = "tx-456" };

        var response = await _service.ConfirmPaymentAsync(reservation.Id, request);

        response.Status.Should().Be("confirmed");
    }

    [Fact]
    public async Task ConfirmPayment_QuandoValido_DeveRetornarSaleIdGerado()
    {
        var reservation = new ReservationBuilder().Build();
        _mockReservationRepository
            .Setup(x => x.GetByIdAsync(reservation.Id))
            .ReturnsAsync(reservation);

        var request = new ConfirmPaymentRequest { PaymentMethod = "credit_card", TransactionId = "tx-456" };

        var response = await _service.ConfirmPaymentAsync(reservation.Id, request);

        response.SaleId.Should().NotBeNullOrEmpty();
        response.SaleId.Should().StartWith("sale-");
    }

    [Fact]
    public async Task ConfirmPayment_QuandoValido_DeveRetornarAssentosCorretos()
    {
        var reservation = new ReservationBuilder().WithSeats("A1", "A2").Build();
        _mockReservationRepository
            .Setup(x => x.GetByIdAsync(reservation.Id))
            .ReturnsAsync(reservation);

        var request = new ConfirmPaymentRequest { PaymentMethod = "credit_card", TransactionId = "tx-456" };

        var response = await _service.ConfirmPaymentAsync(reservation.Id, request);

        response.Seats.Should().BeEquivalentTo(new[] { "A1", "A2" });
    }

    [Fact]
    public async Task ConfirmPayment_QuandoValido_DeveRetornarPaidAtProximoAgora()
    {
        var reservation = new ReservationBuilder().Build();
        _mockReservationRepository
            .Setup(x => x.GetByIdAsync(reservation.Id))
            .ReturnsAsync(reservation);

        var request = new ConfirmPaymentRequest { PaymentMethod = "credit_card", TransactionId = "tx-456" };

        var response = await _service.ConfirmPaymentAsync(reservation.Id, request);

        response.PaidAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task ConfirmPayment_QuandoValido_DevePersistirReservaAtualizada()
    {
        var reservation = new ReservationBuilder().Build();
        _mockReservationRepository
            .Setup(x => x.GetByIdAsync(reservation.Id))
            .ReturnsAsync(reservation);

        var request = new ConfirmPaymentRequest { PaymentMethod = "credit_card", TransactionId = "tx-456" };

        await _service.ConfirmPaymentAsync(reservation.Id, request);

        _mockReservationRepository.Verify(x => x.UpdateAsync(It.IsAny<Reservation>()), Times.Once);
    }
}
