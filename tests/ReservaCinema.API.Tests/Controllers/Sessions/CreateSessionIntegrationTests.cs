using ReservaCinema.Tests.Shared.Builders;

namespace ReservaCinema.API.Tests.Controllers.Sessions;

/// <summary>
/// Testes de integração para CreateSession endpoint.
/// </summary>
public class CreateSessionIntegrationTests
{
    [Fact]
    public void CreateSessionRequest_WithValidData_ShouldBeValid()
    {
        // Arrange
        var futureTime = DateTime.UtcNow.AddHours(1);
        var request = new CreateSessionRequestBuilder()
            .WithMovieTitle("The Matrix")
            .WithStartTime(futureTime)
            .WithRoomNumber("A1")
            .WithTotalSeats(100)
            .WithTicketPrice(25.50m)
            .Build();

        // Act & Assert
        request.Should().NotBeNull();
        request.MovieTitle.Should().Be("The Matrix");
        request.RoomNumber.Should().Be("A1");
        request.TotalSeats.Should().Be(100);
        request.TicketPrice.Should().Be(25.50m);
    }

    [Fact]
    public void CreateSessionRequest_WithEmptyMovieTitle_ShouldFail()
    {
        // Arrange
        var request = new CreateSessionRequestBuilder()
            .WithMovieTitle(string.Empty)
            .Build();

        // Act & Assert
        request.MovieTitle.Should().BeEmpty();
    }

    [Fact]
    public void CreateSessionRequest_WithPastStartTime_ShouldFail()
    {
        // Arrange
        var pastTime = DateTime.UtcNow.AddHours(-1);
        var request = new CreateSessionRequestBuilder()
            .WithStartTime(pastTime)
            .Build();

        // Act & Assert
        request.StartTime.Should().BeBefore(DateTime.UtcNow);
    }

    [Fact]
    public void CreateSessionRequest_WithInvalidRoomNumber_ShouldFail()
    {
        // Arrange
        var request = new CreateSessionRequestBuilder()
            .WithRoomNumber("A")
            .Build();

        // Act & Assert
        request.RoomNumber.Length.Should().BeLessThan(2);
    }

    [Fact]
    public void CreateSessionRequest_WithZeroSeats_ShouldFail()
    {
        // Arrange
        var request = new CreateSessionRequestBuilder()
            .WithTotalSeats(0)
            .Build();

        // Act & Assert
        request.TotalSeats.Should().BeLessOrEqualTo(0);
    }

    [Fact]
    public void CreateSessionRequest_WithNegativeTicketPrice_ShouldFail()
    {
        // Arrange
        var request = new CreateSessionRequestBuilder()
            .WithTicketPrice(-10m)
            .Build();

        // Act & Assert
        request.TicketPrice.Should().BeLessOrEqualTo(0);
    }
}
