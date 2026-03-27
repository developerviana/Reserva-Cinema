using ReservaCinema.Application.DTOs.Sessions;
using ReservaCinema.Tests.Shared.Builders;
using ReservaCinema.Tests.Shared.Constants;

namespace ReservaCinema.Application.Tests.DTOs;

/// <summary>
/// Testes para validação de CreateSessionRequest.
/// </summary>
public class CreateSessionRequestTests
{
    [Fact]
    public void CreateSessionRequest_WithValidData_ShouldBeValid()
    {
        // Arrange
        var request = new CreateSessionRequestBuilder()
            .WithMovieTitle(TestDataConstants.Sessions.ValidMovieTitle)
            .WithRoomNumber(TestDataConstants.Sessions.ValidRoomNumber)
            .WithTotalSeats(TestDataConstants.Sessions.ValidTotalSeats)
            .WithTicketPrice(TestDataConstants.Sessions.ValidTicketPrice)
            .WithStartTime(TestDataConstants.Sessions.ValidStartTime)
            .Build();

        // Act & Assert
        request.Should().NotBeNull();
        request.MovieTitle.Should().NotBeEmpty();
        request.RoomNumber.Should().NotBeEmpty();
        request.TotalSeats.Should().BeGreaterThan(0);
        request.TicketPrice.Should().BeGreaterThan(0);
        request.StartTime.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public void CreateSessionRequest_WithEmptyMovieTitle_ShouldBeInvalid()
    {
        // Arrange & Act
        var request = new CreateSessionRequestBuilder()
            .WithMovieTitle(TestDataConstants.Sessions.InvalidMovieTitle)
            .Build();

        // Assert
        request.MovieTitle.Should().BeEmpty();
    }

    [Fact]
    public void CreateSessionRequest_WithInvalidRoomNumber_ShouldBeInvalid()
    {
        // Arrange & Act
        var request = new CreateSessionRequestBuilder()
            .WithRoomNumber(TestDataConstants.Sessions.InvalidRoomNumber)
            .Build();

        // Assert
        request.RoomNumber.Length.Should().BeLessThan(2);
    }

    [Fact]
    public void CreateSessionRequest_WithZeroSeats_ShouldBeInvalid()
    {
        // Arrange & Act
        var request = new CreateSessionRequestBuilder()
            .WithTotalSeats(TestDataConstants.Sessions.InvalidTotalSeats)
            .Build();

        // Assert
        request.TotalSeats.Should().BeLessOrEqualTo(0);
    }

    [Fact]
    public void CreateSessionRequest_WithNegativeTicketPrice_ShouldBeInvalid()
    {
        // Arrange & Act
        var request = new CreateSessionRequestBuilder()
            .WithTicketPrice(TestDataConstants.Sessions.InvalidTicketPrice)
            .Build();

        // Assert
        request.TicketPrice.Should().BeLessOrEqualTo(0);
    }

    [Fact]
    public void CreateSessionRequest_WithPastStartTime_ShouldBeInvalid()
    {
        // Arrange & Act
        var request = new CreateSessionRequestBuilder()
            .WithStartTime(TestDataConstants.Sessions.InvalidStartTimePast)
            .Build();

        // Assert
        request.StartTime.Should().BeBefore(DateTime.UtcNow);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void CreateSessionRequest_WithVariousInvalidMovieTitles_ShouldBeInvalid(string? movieTitle)
    {
        // Arrange & Act
        var request = new CreateSessionRequestBuilder()
            .WithMovieTitle(movieTitle ?? "")
            .Build();

        // Assert
        string.IsNullOrWhiteSpace(request.MovieTitle).Should().BeTrue();
    }
}
