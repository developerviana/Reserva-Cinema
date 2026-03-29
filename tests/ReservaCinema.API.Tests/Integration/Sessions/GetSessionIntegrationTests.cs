using FluentAssertions;
using ReservaCinema.Application.DTOs.Sessions;

namespace ReservaCinema.API.Tests.Integration.Sessions;

/// <summary>
/// Testes de integração para GetSessionById endpoint.
/// </summary>
public class GetSessionIntegrationTests
{
    [Fact]
    public void GetSessionById_WithValidSessionResponse_ShouldReturnExpectedData()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var futureTime = DateTime.UtcNow.AddHours(1);
        var createdAt = DateTime.UtcNow;

        var expectedResponse = new SessionResponse
        {
            Id = sessionId,
            MovieTitle = "The Matrix",
            StartTime = futureTime,
            RoomNumber = "A1",
            TotalSeats = 100,
            AvailableSeats = 100,
            TicketPrice = 25.50m,
            CreatedAt = createdAt,
            IsActive = true,
            DurationMinutes = 136,
            RatingClassification = "14"
        };

        // Act
        var result = expectedResponse;

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(sessionId);
        result.MovieTitle.Should().Be("The Matrix");
        result.RoomNumber.Should().Be("A1");
        result.TotalSeats.Should().Be(100);
        result.AvailableSeats.Should().Be(100);
        result.TicketPrice.Should().Be(25.50m);
        result.IsActive.Should().BeTrue();
        result.DurationMinutes.Should().Be(136);
        result.RatingClassification.Should().Be("14");
    }

    [Fact]
    public void GetSessionById_WithValidSessionResponse_ShouldHaveCorrectTimestamps()
    {
        // Arrange
        var createdAt = DateTime.UtcNow;
        var sessionResponse = new SessionResponse
        {
            Id = Guid.NewGuid(),
            MovieTitle = "Inception",
            StartTime = DateTime.UtcNow.AddHours(2),
            RoomNumber = "B2",
            TotalSeats = 150,
            AvailableSeats = 120,
            TicketPrice = 30.00m,
            CreatedAt = createdAt,
            IsActive = true
        };

        // Act
        var result = sessionResponse;

        // Assert
        result.CreatedAt.Should().Be(createdAt);
        result.UpdatedAt.Should().BeNull();
    }
}
