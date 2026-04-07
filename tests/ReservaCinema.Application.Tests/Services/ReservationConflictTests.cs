using FluentAssertions;
using ReservaCinema.Application.DTOs.Reservations;
using ReservaCinema.Application.Services;
using ReservaCinema.Domain.Exceptions;

namespace ReservaCinema.Application.Tests.Services;

/// <summary>
/// Testes unitários para conflitos de assentos em ReservationService.
/// Validam o cenário onde assentos já estão reservados.
/// </summary>
public class ReservationConflictTests
{
    private readonly ReservationService _service = new();

    [Fact]
    public async Task CreateReservation_WhenSeatsAreAlreadyReserved_ShouldThrowSeatAlreadyReservedException()
    {
        // Arrange - cria primeira reserva com assentos A1 e A2
        var sessionId = Guid.NewGuid();
        var request1 = new CreateReservationRequest
        {
            SessionId = sessionId,
            UserId = "user-1",
            SeatNumbers = new[] { "A1", "A2" }
        };

        await _service.CreateReservationAsync(request1);

        // Tenta criar segunda reserva com os mesmos assentos
        var request2 = new CreateReservationRequest
        {
            SessionId = sessionId,
            UserId = "user-2",
            SeatNumbers = new[] { "A1", "A3" }  // A1 já está reservado
        };

        // Act & Assert
        await FluentActions.Invoking(() => _service.CreateReservationAsync(request2))
            .Should()
            .ThrowAsync<SeatAlreadyReservedException>();
    }

    [Fact]
    public async Task CreateReservation_WhenMultipleSeatsAreReserved_ShouldThrowWithConflictingSeats()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var request1 = new CreateReservationRequest
        {
            SessionId = sessionId,
            UserId = "user-1",
            SeatNumbers = new[] { "B1", "B2", "B3" }
        };

        await _service.CreateReservationAsync(request1);

        var request2 = new CreateReservationRequest
        {
            SessionId = sessionId,
            UserId = "user-2",
            SeatNumbers = new[] { "B2", "B4", "B5" }  // B2 já está reservado
        };

        // Act & Assert
        var exception = await FluentActions.Invoking(() => _service.CreateReservationAsync(request2))
            .Should()
            .ThrowAsync<SeatAlreadyReservedException>();

        exception.And.ConflictingSeats.Should().Contain("B2");
        exception.And.ConflictingSeats.Should().NotContain("B4");
    }

    [Fact]
    public async Task CreateReservation_WhenAllSeatsAreReserved_ShouldThrowWithAllConflictingSeats()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var request1 = new CreateReservationRequest
        {
            SessionId = sessionId,
            UserId = "user-1",
            SeatNumbers = new[] { "C1", "C2" }
        };

        await _service.CreateReservationAsync(request1);

        var request2 = new CreateReservationRequest
        {
            SessionId = sessionId,
            UserId = "user-2",
            SeatNumbers = new[] { "C1", "C2" }  // Todos os assentos estão reservados
        };

        // Act & Assert
        var exception = await FluentActions.Invoking(() => _service.CreateReservationAsync(request2))
            .Should()
            .ThrowAsync<SeatAlreadyReservedException>();

        exception.And.ConflictingSeats.Should().HaveCount(2);
        exception.And.ConflictingSeats.Should().Contain("C1");
        exception.And.ConflictingSeats.Should().Contain("C2");
    }
}
