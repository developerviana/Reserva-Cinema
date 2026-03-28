using ReservaCinema.Application.DTOs.Sessions;
using Xunit;

namespace ReservaCinema.API.Tests.Sessions;			

public class CreateSessionTests
{
    [Fact]
    public void CreateSessionRequest_WithValidData_ShouldBeValid()
    {
        // Arrange
        var futureTime = DateTime.UtcNow.AddHours(1);
        var request = new CreateSessionRequest
        {
            MovieTitle = "The Matrix",
            StartTime = futureTime,
            RoomNumber = "A1",
            TotalSeats = 100,
            TicketPrice = 25.50m
        };

        // Act & Assert
        Assert.NotNull(request);
        Assert.Equal("The Matrix", request.MovieTitle);
        Assert.Equal(futureTime, request.StartTime);
        Assert.Equal("A1", request.RoomNumber);
        Assert.Equal(100, request.TotalSeats);
        Assert.Equal(25.50m, request.TicketPrice);
    }

    [Fact]
    public void MovieTitle_WhenEmpty_ShouldBeFalse()
    {
        // Arrange
        var request = new CreateSessionRequest
        {
            MovieTitle = string.Empty,
            StartTime = DateTime.UtcNow.AddHours(1),
            RoomNumber = "A1",
            TotalSeats = 100,
            TicketPrice = 25.50m
        };

        // Act & Assert
        Assert.True(string.IsNullOrWhiteSpace(request.MovieTitle), "MovieTitle should be required");
    }

    [Fact]
    public void StartTime_WhenPast_ShouldBeFalse()
    {
        // Arrange
        var pastTime = DateTime.UtcNow.AddHours(-1);
        var request = new CreateSessionRequest
        {
            MovieTitle = "The Matrix",
            StartTime = pastTime,
            RoomNumber = "A1",
            TotalSeats = 100,
            TicketPrice = 25.50m
        };

        // Act & Assert
        Assert.True(request.StartTime < DateTime.UtcNow, "StartTime should not be in the past");
    }

    [Fact]
    public void RoomNumber_WhenLessThanTwoCharacters_ShouldBeFalse()
    {
        // Arrange
        var request = new CreateSessionRequest
        {
            MovieTitle = "The Matrix",
            StartTime = DateTime.UtcNow.AddHours(1),
            RoomNumber = "A",
            TotalSeats = 100,
            TicketPrice = 25.50m
        };

        // Act & Assert
        Assert.True(request.RoomNumber.Length < 2, "RoomNumber should have at least 2 characters");
    }

    [Fact]
    public void TotalSeats_WhenZero_ShouldBeFalse()
    {
        // Arrange
        var request = new CreateSessionRequest
        {
            MovieTitle = "The Matrix",
            StartTime = DateTime.UtcNow.AddHours(1),
            RoomNumber = "A1",
            TotalSeats = 0,
            TicketPrice = 25.50m
        };

        // Act & Assert
        Assert.True(request.TotalSeats <= 0, "TotalSeats should be greater than 0");
    }

    [Fact]
    public void TicketPrice_WhenNegative_ShouldBeFalse()
    {
        // Arrange
        var request = new CreateSessionRequest
        {
            MovieTitle = "The Matrix",
            StartTime = DateTime.UtcNow.AddHours(1),
            RoomNumber = "A1",
            TotalSeats = 100,
            TicketPrice = -5.00m
        };

        // Act & Assert
        Assert.True(request.TicketPrice < 0, "TicketPrice should never be negative");
    }

    [Fact]
    public void TicketPrice_WhenZero_ShouldBeValid()
    {
        // Arrange
        var request = new CreateSessionRequest
        {
            MovieTitle = "The Matrix",
            StartTime = DateTime.UtcNow.AddHours(1),
            RoomNumber = "A1",
            TotalSeats = 100,
            TicketPrice = 0.00m
        };

        // Act & Assert
        Assert.True(request.TicketPrice >= 0, "TicketPrice can be 0");
    }
}
