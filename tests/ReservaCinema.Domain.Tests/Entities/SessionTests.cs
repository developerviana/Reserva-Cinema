using ReservaCinema.Domain.Entities;

namespace ReservaCinema.Domain.Tests.Entities;

/// <summary>
/// Testes unitários para a entidade Session.
/// </summary>
public class SessionTests
{
    [Fact]
    public void Session_WhenCreatedWithValidData_ShouldHaveCorrectProperties()
    {
        // Arrange
        var session = new Session
        {
            MovieTitle = "The Matrix",
            RoomNumber = "A1",
            TotalSeats = 100,
            TicketPrice = 25.50m
        };

        // Act & Assert
        session.MovieTitle.Should().Be("The Matrix");
        session.RoomNumber.Should().Be("A1");
        session.TotalSeats.Should().Be(100);
        session.TicketPrice.Should().Be(25.50m);
    }

    [Fact]
    public void Session_WhenCreated_ShouldHaveUniqueId()
    {
        // Arrange & Act
        var session1 = new Session { MovieTitle = "Movie 1", RoomNumber = "A1", TotalSeats = 100, TicketPrice = 25m };
        var session2 = new Session { MovieTitle = "Movie 2", RoomNumber = "A2", TotalSeats = 100, TicketPrice = 25m };

        // Assert
        session1.Id.Should().NotBe(session2.Id);
    }

    [Fact]
    public void Session_WithFutureStartTime_ShouldBeValid()
    {
        // Arrange
        var futureTime = DateTime.UtcNow.AddHours(2);
        var session = new Session
        {
            StartTime = futureTime,
            MovieTitle = "Movie",
            RoomNumber = "A1",
            TotalSeats = 100,
            TicketPrice = 25m
        };

        // Act & Assert
        session.StartTime.Should().BeAfter(DateTime.UtcNow);
    }

    [Theory]
    [InlineData(50)]
    [InlineData(100)]
    [InlineData(200)]
    public void Session_WithVariousSeatsCount_ShouldAcceptAllPositiveValues(int seatsCount)
    {
        // Arrange & Act
        var session = new Session
        {
            MovieTitle = "Movie",
            RoomNumber = "A1",
            TotalSeats = seatsCount,
            TicketPrice = 25m
        };

        // Assert
        session.TotalSeats.Should().Be(seatsCount);
    }
}
