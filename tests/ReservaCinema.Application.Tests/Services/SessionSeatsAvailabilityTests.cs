using ReservaCinema.Application.DTOs.Sessions;
using ReservaCinema.Application.Services;

namespace ReservaCinema.Application.Tests.Services;

/// <summary>
/// Testes unitários para GetSessionSeatsAsync do SessionService.
/// Testam a disponibilidade em tempo real de assentos.
/// </summary>
public class SessionSeatsAvailabilityTests
{
    private readonly SessionServiceMock _service;

    public SessionSeatsAvailabilityTests()
    {
        _service = new SessionServiceMock();
    }

    [Fact]
    public async Task GetSessionSeats_WithValidSessionId_ShouldReturnSeatsInfo()
    {
        // Arrange - cria uma sessão
        var createRequest = new CreateSessionRequest
        {
            MovieTitle = "Oppenheimer",
            StartTime = DateTime.UtcNow.AddHours(2),
            RoomNumber = "A1",
            TotalSeats = 4,
            TicketPrice = 25.00m
        };

        var createdSession = await _service.CreateSessionAsync(createRequest);

        // Act
        var result = await _service.GetSessionSeatsAsync(createdSession.Id);

        // Assert
        result.Should().NotBeNull();
        result.SessionId.Should().Be(createdSession.Id);
        result.TotalSeats.Should().Be(4);
        result.AvailableSeats.Should().Be(4);
        result.Seats.Should().HaveCount(4);
    }

    [Fact]
    public async Task GetSessionSeats_WithInvalidSessionId_ShouldReturnNull()
    {
        // Act
        var result = await _service.GetSessionSeatsAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetSessionSeats_AllNewSessionSeatsAreAvailable()
    {
        // Arrange
        var createRequest = new CreateSessionRequest
        {
            MovieTitle = "Inception",
            StartTime = DateTime.UtcNow.AddHours(1),
            RoomNumber = "B1",
            TotalSeats = 3,
            TicketPrice = 30.00m
        };

        var createdSession = await _service.CreateSessionAsync(createRequest);

        // Act
        var result = await _service.GetSessionSeatsAsync(createdSession.Id);

        // Assert
        result.Seats.Should().AllSatisfy(seat => 
            seat.Status.Should().Be(SeatStatus.Available)
        );
    }

    [Fact]
    public async Task GetSessionSeats_SeatNumbersShouldFollowStandardNaming()
    {
        // Arrange
        var createRequest = new CreateSessionRequest
        {
            MovieTitle = "Inception",
            StartTime = DateTime.UtcNow.AddHours(1),
            RoomNumber = "A1",
            TotalSeats = 2,
            TicketPrice = 25.00m
        };

        var createdSession = await _service.CreateSessionAsync(createRequest);

        // Act
        var result = await _service.GetSessionSeatsAsync(createdSession.Id);

        // Assert
        result.Seats[0].Number.Should().Be("A11");
        result.Seats[1].Number.Should().Be("A12");
    }

    [Fact]
    public async Task GetSessionSeats_WithReservedSeats_ShouldShowReservedStatus()
    {
        // Arrange
        var createRequest = new CreateSessionRequest
        {
            MovieTitle = "Matrix",
            StartTime = DateTime.UtcNow.AddHours(1),
            RoomNumber = "C",
            TotalSeats = 2,
            TicketPrice = 25.00m
        };

        var createdSession = await _service.CreateSessionAsync(createRequest);

        // Marca alguns assentos como reservados (simulado)
        _service.ReserveSeat(createdSession.Id, "C1");

        // Act
        var result = await _service.GetSessionSeatsAsync(createdSession.Id);

        // Assert
        result.AvailableSeats.Should().Be(1);
        result.Seats.Should().Contain(s => s.Number == "C1" && s.Status == SeatStatus.Reserved);
    }

    [Fact]
    public async Task GetSessionSeats_ShouldNotIncludeExpiresAtForAvailableSeats()
    {
        // Arrange
        var createRequest = new CreateSessionRequest
        {
            MovieTitle = "Dune",
            StartTime = DateTime.UtcNow.AddHours(1),
            RoomNumber = "D",
            TotalSeats = 1,
            TicketPrice = 25.00m
        };

        var createdSession = await _service.CreateSessionAsync(createRequest);

        // Act
        var result = await _service.GetSessionSeatsAsync(createdSession.Id);

        // Assert
        result.Seats.Where(s => s.Status == SeatStatus.Available)
            .Should().AllSatisfy(s => s.ExpiresAt.Should().BeNull());
    }

    [Fact]
    public async Task GetSessionSeats_ShouldIncludeExpiresAtForReservedSeats()
    {
        // Arrange
        var createRequest = new CreateSessionRequest
        {
            MovieTitle = "Interstellar",
            StartTime = DateTime.UtcNow.AddHours(1),
            RoomNumber = "E",
            TotalSeats = 1,
            TicketPrice = 25.00m
        };

        var createdSession = await _service.CreateSessionAsync(createRequest);
        _service.ReserveSeat(createdSession.Id, "E1");

        // Act
        var result = await _service.GetSessionSeatsAsync(createdSession.Id);

        // Assert
        result.Seats.Where(s => s.Status == SeatStatus.Reserved)
            .Should().AllSatisfy(s => s.ExpiresAt.Should().NotBeNull());
    }
}
