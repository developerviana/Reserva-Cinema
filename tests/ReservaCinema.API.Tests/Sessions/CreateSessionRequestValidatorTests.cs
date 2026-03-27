using ReservaCinema.Application.DTOs.Sessions;
using ReservaCinema.Application.Validators.Sessions;
using Xunit;

namespace ReservaCinema.API.Tests.Sessions;

public class CreateSessionRequestValidatorTests
{
    [Fact]
    public void Validate_WithValidData_ReturnsNoErrors()
    {
        // Arrange
        var futureTime = DateTime.UtcNow.AddHours(1);
        var validator = new CreateSessionRequestValidator();
        var request = new CreateSessionRequest
        {
            MovieTitle = "The Matrix",
            StartTime = futureTime,
            RoomNumber = "A1",
            TotalSeats = 100,
            TicketPrice = 25.50
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.True(result.IsValid, "Should have no validation errors");
    }

    [Fact]
    public void Validate_MovieTitleIsEmpty_ReturnsError()
    {
        // Arrange
        var validator = new CreateSessionRequestValidator();
        var request = new CreateSessionRequest
        {
            MovieTitle = string.Empty,
            StartTime = DateTime.UtcNow.AddHours(1),
            RoomNumber = "A1",
            TotalSeats = 100,
            TicketPrice = 25.50
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("MovieTitle", result.Errors.Select(e => e.PropertyName));
    }

    [Fact]
    public void Validate_MovieTitleIsNull_ReturnsError()
    {
        // Arrange
        var validator = new CreateSessionRequestValidator();
        var request = new CreateSessionRequest
        {
            MovieTitle = null,
            StartTime = DateTime.UtcNow.AddHours(1),
            RoomNumber = "A1",
            TotalSeats = 100,
            TicketPrice = 25.50
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("MovieTitle", result.Errors.Select(e => e.PropertyName));
    }

    [Fact]
    public void Validate_StartTimeIsInThePast_ReturnsError()
    {
        // Arrange
        var validator = new CreateSessionRequestValidator();
        var request = new CreateSessionRequest
        {
            MovieTitle = "The Matrix",
            StartTime = DateTime.UtcNow.AddHours(-1),
            RoomNumber = "A1",
            TotalSeats = 100,
            TicketPrice = 25.50
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("StartTime", result.Errors.Select(e => e.PropertyName));
    }

    [Fact]
    public void Validate_RoomNumberHasLessThanTwoCharacters_ReturnsError()
    {
        // Arrange
        var validator = new CreateSessionRequestValidator();
        var request = new CreateSessionRequest
        {
            MovieTitle = "The Matrix",
            StartTime = DateTime.UtcNow.AddHours(1),
            RoomNumber = "A",
            TotalSeats = 100,
            TicketPrice = 25.50
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("RoomNumber", result.Errors.Select(e => e.PropertyName));
    }

    [Fact]
    public void Validate_RoomNumberIsEmpty_ReturnsError()
    {
        // Arrange
        var validator = new CreateSessionRequestValidator();
        var request = new CreateSessionRequest
        {
            MovieTitle = "The Matrix",
            StartTime = DateTime.UtcNow.AddHours(1),
            RoomNumber = string.Empty,
            TotalSeats = 100,
            TicketPrice = 25.50
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("RoomNumber", result.Errors.Select(e => e.PropertyName));
    }

    [Fact]
    public void Validate_TotalSeatsIsZero_ReturnsError()
    {
        // Arrange
        var validator = new CreateSessionRequestValidator();
        var request = new CreateSessionRequest
        {
            MovieTitle = "The Matrix",
            StartTime = DateTime.UtcNow.AddHours(1),
            RoomNumber = "A1",
            TotalSeats = 0,
            TicketPrice = 25.50
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("TotalSeats", result.Errors.Select(e => e.PropertyName));
    }

    [Fact]
    public void Validate_TotalSeatsIsNegative_ReturnsError()
    {
        // Arrange
        var validator = new CreateSessionRequestValidator();
        var request = new CreateSessionRequest
        {
            MovieTitle = "The Matrix",
            StartTime = DateTime.UtcNow.AddHours(1),
            RoomNumber = "A1",
            TotalSeats = -10,
            TicketPrice = 25.50
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("TotalSeats", result.Errors.Select(e => e.PropertyName));
    }

    [Fact]
    public void Validate_TicketPriceIsNegative_ReturnsError()
    {
        // Arrange
        var validator = new CreateSessionRequestValidator();
        var request = new CreateSessionRequest
        {
            MovieTitle = "The Matrix",
            StartTime = DateTime.UtcNow.AddHours(1),
            RoomNumber = "A1",
            TotalSeats = 100,
            TicketPrice = -5.00
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("TicketPrice", result.Errors.Select(e => e.PropertyName));
    }

    [Fact]
    public void Validate_TicketPriceIsZero_IsValid()
    {
        // Arrange
        var validator = new CreateSessionRequestValidator();
        var request = new CreateSessionRequest
        {
            MovieTitle = "The Matrix",
            StartTime = DateTime.UtcNow.AddHours(1),
            RoomNumber = "A1",
            TotalSeats = 100,
            TicketPrice = 0.00
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.True(result.IsValid, "TicketPrice can be 0");
    }
}
