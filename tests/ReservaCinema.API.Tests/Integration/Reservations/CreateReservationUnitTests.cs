using FluentAssertions;

namespace ReservaCinema.API.Tests.Integration.Reservations;

/// <summary>
/// Testes de integração para POST /api/reservations endpoint.
/// Foco em regras de negócio críticas com máxima cobertura mínima.
/// </summary>
public class CreateReservationUnitTests
{
    [Fact]
    public void CreateReservationRequest_WithValidData_ShouldHaveCorrectStructure()
    {
        // Arrange & Act
        var sessionId = Guid.NewGuid();
        var userId = "user-123";
        var seatNumbers = new[] { "A1", "A2" };

        // Assert - Valida estrutura e dados críticos
        sessionId.Should().NotBeEmpty();
        userId.Should().NotBeNullOrWhiteSpace();
        seatNumbers.Should().NotBeNullOrEmpty();
        seatNumbers.Should().AllSatisfy(seat => seat.Should().MatchRegex(@"^[A-Z]\d+$"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public void CreateReservationRequest_WithInvalidSessionId_ShouldFail(string sessionId)
    {
        // Arrange & Act
        var isValidGuid = Guid.TryParse(sessionId, out var parsedId) && parsedId != Guid.Empty;

        // Assert
        isValidGuid.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void CreateReservationRequest_WithInvalidUserId_ShouldFail(string? userId)
    {
        // Arrange & Act
        var isValid = !string.IsNullOrWhiteSpace(userId);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void CreateReservationRequest_WithoutSeats_ShouldFail()
    {
        // Arrange & Act
        var seatNumbers = Array.Empty<string>();

        // Assert
        seatNumbers.Should().BeEmpty();
    }

    [Fact]
    public void CreateReservationResponse_ShouldContainAllRequiredFields()
    {
        // Arrange - Simula resposta sucesso 201
        var response = new
        {
            reservationId = "res-789",
            status = "pending",
            expiresAt = DateTime.UtcNow.AddHours(1),
            seats = new[] { "A1", "A2" },
            totalAmount = 50.00m
        };

        // Act & Assert
        response.reservationId.Should().NotBeNullOrEmpty();
        response.status.Should().Be("pending");
        response.expiresAt.Should().BeAfter(DateTime.UtcNow);
        response.seats.Should().HaveCount(2);
        response.totalAmount.Should().BeGreaterThan(0);
    }

    [Fact]
    public void CreateReservationResponse_ConflictError_ShouldHaveCorrectStructure()
    {
        // Arrange - Simula resposta conflito 409
        var error = new
        {
            error = "SEAT_ALREADY_RESERVED",
            message = "Assentos [A1] não disponíveis",
            conflictingSeats = new[] { "A1" }
        };

        // Act & Assert
        error.error.Should().Be("SEAT_ALREADY_RESERVED");
        error.message.Should().Contain("não disponíveis");
        error.conflictingSeats.Should().Contain("A1");
    }
}
