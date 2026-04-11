using FluentAssertions;
using Moq;
using ReservaCinema.Application.DTOs.Reservations;
using ReservaCinema.Application.Services;

namespace ReservaCinema.Application.Tests.Services;

/// <summary>
/// Testes unitários para ReservationService.
/// Testam regras de negócio em isolamento (sem banco de dados).
/// Os testes antigos foram preservados com mock do IDistributedLockService.
/// </summary>
public class ReservationServiceTests
{
    private readonly Mock<IDistributedLockService> _mockLockService;
    private readonly ReservationService _service;

    public ReservationServiceTests()
    {
        _mockLockService = new Mock<IDistributedLockService>();
        // Por padrão, o mock retorna sucesso na aquisição de lock
        _mockLockService
            .Setup(x => x.AcquireLockAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .ReturnsAsync(Guid.NewGuid().ToString());
        _mockLockService
            .Setup(x => x.ReleaseLockAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        _service = new ReservationService(_mockLockService.Object);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateReservation_WithInvalidInput_ShouldThrowArgumentException(string userId)
    {
        var request = new CreateReservationRequest { SessionId = Guid.NewGuid(), UserId = userId, SeatNumbers = new[] { "A1" } };
        await FluentActions.Invoking(() => _service.CreateReservationAsync(request)).Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateReservation_WithEmptySessionId_ShouldThrowArgumentException()
    {
        var request = new CreateReservationRequest { SessionId = Guid.Empty, UserId = "user-1", SeatNumbers = new[] { "A1" } };
        await FluentActions.Invoking(() => _service.CreateReservationAsync(request)).Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateReservation_WithEmptySeats_ShouldThrowArgumentException()
    {
        var request = new CreateReservationRequest { SessionId = Guid.NewGuid(), UserId = "user-1", SeatNumbers = Array.Empty<string>() };
        await FluentActions.Invoking(() => _service.CreateReservationAsync(request)).Should().ThrowAsync<ArgumentException>();
    }

    [Theory]
    [InlineData(1, 25.50)]
    [InlineData(2, 51.00)]
    [InlineData(3, 76.50)]
    [InlineData(5, 127.50)]
    public async Task CreateReservation_ShouldCalculatePriceCorrectly(int seatCount, decimal expectedPrice)
    {
        var seats = Enumerable.Range(1, seatCount).Select(i => $"A{i}").ToArray();
        var request = new CreateReservationRequest { SessionId = Guid.NewGuid(), UserId = "user-1", SeatNumbers = seats };
        var response = await _service.CreateReservationAsync(request);
        response.TotalAmount.Should().Be(expectedPrice);
    }

    [Fact]
    public async Task CreateReservation_ShouldReturnValidResponse()
    {
        var seats = new[] { "A1", "A2" };
        var request = new CreateReservationRequest { SessionId = Guid.NewGuid(), UserId = "user-1", SeatNumbers = seats };
        var response = await _service.CreateReservationAsync(request);

        response.ReservationId.Should().StartWith("res-").And.HaveLength(12);
        response.Status.Should().Be("pending");
        response.Seats.Should().BeEquivalentTo(seats);
        response.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddHours(1), TimeSpan.FromSeconds(2));
    }

    // ===== RED TESTS - Não implementados ainda =====

    [Fact]
    public async Task CreateReservation_WithDuplicateSeats_ShouldThrowArgumentException()
    {
        var request = new CreateReservationRequest { SessionId = Guid.NewGuid(), UserId = "user-1", SeatNumbers = new[] { "A1", "A2", "A1" } };
        await FluentActions.Invoking(() => _service.CreateReservationAsync(request)).Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateReservation_WithMoreThan10Seats_ShouldThrowArgumentException()
    {
        var seats = Enumerable.Range(1, 11).Select(i => $"A{i}").ToArray();
        var request = new CreateReservationRequest { SessionId = Guid.NewGuid(), UserId = "user-1", SeatNumbers = seats };
        await FluentActions.Invoking(() => _service.CreateReservationAsync(request)).Should().ThrowAsync<ArgumentException>();
    }
}
