using ReservaCinema.Application.DTOs.Sessions;
using ReservaCinema.Tests.Shared.Builders;

namespace ReservaCinema.API.Tests.Unit.DTOs;

/// <summary>
/// Testes unitários para CreateSessionRequest DTO.
/// </summary>
public class CreateSessionRequestDTOTests
{
    [Fact]
    public void CreateSessionRequest_WhenInitialized_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var request = new CreateSessionRequest();

        // Assert
        request.MovieTitle.Should().BeNullOrEmpty();
        request.RoomNumber.Should().BeNullOrEmpty();
        request.TotalSeats.Should().Be(0);
        request.TicketPrice.Should().Be(0m);
    }

    [Fact]
    public void CreateSessionRequest_WhenPropertiesSet_ShouldReflectChanges()
    {
        // Arrange
        var futureTime = DateTime.UtcNow.AddHours(2);
        var request = new CreateSessionRequest
        {
            MovieTitle = "Inception",
            StartTime = futureTime,
            RoomNumber = "B2",
            TotalSeats = 150,
            TicketPrice = 30.00m
        };

        // Act & Assert
        request.MovieTitle.Should().Be("Inception");
        request.RoomNumber.Should().Be("B2");
        request.TotalSeats.Should().Be(150);
        request.TicketPrice.Should().Be(30.00m);
        request.StartTime.Should().Be(futureTime);
    }

    [Theory]
    [InlineData("Movie A", "A1")]
    [InlineData("Movie B", "B2")]
    [InlineData("Movie C", "C3")]
    public void CreateSessionRequest_WithDifferentMoviesAndRooms_ShouldAcceptAll(
        string movieTitle, 
        string roomNumber)
    {
        // Arrange & Act
        var request = new CreateSessionRequestBuilder()
            .WithMovieTitle(movieTitle)
            .WithRoomNumber(roomNumber)
            .Build();

        // Assert
        request.MovieTitle.Should().Be(movieTitle);
        request.RoomNumber.Should().Be(roomNumber);
    }
}
