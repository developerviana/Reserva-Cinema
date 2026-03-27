using ReservaCinema.Domain.Entities;
using ReservaCinema.Tests.Shared.Builders;

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
        var sessionBuilder = new SessionBuilder()
            .WithMovieTitle("The Matrix")
            .WithRoomNumber("A1")
            .WithTotalSeats(100)
            .WithTicketPrice(25.50m);

        // Act
        var session = sessionBuilder.Build();

        // Assert
        session.MovieTitle.Should().Be("The Matrix");
        session.RoomNumber.Should().Be("A1");
        session.TotalSeats.Should().Be(100);
        session.TicketPrice.Should().Be(25.50m);
        session.Id.Should().NotBeEmpty();
        session.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Session_WhenCreated_ShouldHaveUniqueId()
    {
        // Arrange & Act
        var session1 = new SessionBuilder().Build();
        var session2 = new SessionBuilder().Build();

        // Assert
        session1.Id.Should().NotBe(session2.Id);
    }

    [Fact]
    public void Session_WithFutureStartTime_ShouldBeValid()
    {
        // Arrange
        var futureTime = DateTime.UtcNow.AddHours(2);
        var session = new SessionBuilder()
            .WithStartTime(futureTime)
            .Build();

        // Act & Assert
        session.StartTime.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public void Session_WithNegativeTicketPrice_ShouldNotBeAllowed()
    {
        // Arrange
        var session = new SessionBuilder()
            .WithTicketPrice(-10m)
            .Build();

        // Act & Assert
        session.TicketPrice.Should().BeLessThan(0);
    }

    [Theory]
    [InlineData(50)]
    [InlineData(100)]
    [InlineData(200)]
    public void Session_WithVariousSeatsCount_ShouldAcceptAllPositiveValues(int seatsCount)
    {
        // Arrange & Act
        var session = new SessionBuilder()
            .WithTotalSeats(seatsCount)
            .Build();

        // Assert
        session.TotalSeats.Should().Be(seatsCount);
    }
}
