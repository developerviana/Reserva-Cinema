using Microsoft.AspNetCore.Mvc;
using Xunit;
using ReservaCinema.API.Controllers;
using ReservaCinema.Application.DTOs.Sessions;

namespace ReservaCinema.API.Tests.Controllers;

public class SessionsControllerTests
{
    private readonly SessionsController _controller;

    public SessionsControllerTests()
    {
        _controller = new SessionsController();
    }

    [Fact]
    public void CreateSession_WithValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var futureTime = DateTime.UtcNow.AddHours(2);
        var request = new CreateSessionRequest
        {
            MovieTitle = "Inception",
            StartTime = futureTime,
            RoomNumber = "A1",
            TotalSeats = 150,
            TicketPrice = 45.50
        };

        // Act
        var result = _controller.CreateSession(request);

        // Assert
        Assert.IsType<CreatedAtActionResult>(result);
        var createdResult = result as CreatedAtActionResult;
        Assert.NotNull(createdResult);
        Assert.Equal(nameof(_controller.CreateSession), createdResult.ActionName);
    }

    [Fact]
    public void CreateSession_WithEmptyMovieTitle_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateSessionRequest
        {
            MovieTitle = string.Empty,
            StartTime = DateTime.UtcNow.AddHours(2),
            RoomNumber = "A1",
            TotalSeats = 150,
            TicketPrice = 45.50
        };

        // Act
        var result = _controller.CreateSession(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateSession_WithPastStartTime_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateSessionRequest
        {
            MovieTitle = "Inception",
            StartTime = DateTime.UtcNow.AddHours(-1),
            RoomNumber = "A1",
            TotalSeats = 150,
            TicketPrice = 45.50
        };

        // Act
        var result = _controller.CreateSession(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateSession_WithRoomNumberLessThanTwoChars_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateSessionRequest
        {
            MovieTitle = "Inception",
            StartTime = DateTime.UtcNow.AddHours(2),
            RoomNumber = "A",
            TotalSeats = 150,
            TicketPrice = 45.50
        };

        // Act
        var result = _controller.CreateSession(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateSession_WithZeroTotalSeats_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateSessionRequest
        {
            MovieTitle = "Inception",
            StartTime = DateTime.UtcNow.AddHours(2),
            RoomNumber = "A1",
            TotalSeats = 0,
            TicketPrice = 45.50
        };

        // Act
        var result = _controller.CreateSession(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateSession_WithNegativeTicketPrice_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateSessionRequest
        {
            MovieTitle = "Inception",
            StartTime = DateTime.UtcNow.AddHours(2),
            RoomNumber = "A1",
            TotalSeats = 150,
            TicketPrice = -10.00
        };

        // Act
        var result = _controller.CreateSession(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateSession_WithZeroTicketPrice_ReturnsCreatedAtAction()
    {
        // Arrange
        var request = new CreateSessionRequest
        {
            MovieTitle = "Inception",
            StartTime = DateTime.UtcNow.AddHours(2),
            RoomNumber = "A1",
            TotalSeats = 150,
            TicketPrice = 0.00
        };

        // Act
        var result = _controller.CreateSession(request);

        // Assert
        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public void CreateSession_WithInvalidData_ReturnsBadRequestWithErrors()
    {
        // Arrange
        var request = new CreateSessionRequest
        {
            MovieTitle = string.Empty,
            StartTime = DateTime.UtcNow.AddHours(-1),
            RoomNumber = "A",
            TotalSeats = -5,
            TicketPrice = -20.00
        };

        // Act
        var result = _controller.CreateSession(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        var badRequestResult = result as BadRequestObjectResult;
        Assert.NotNull(badRequestResult?.Value);
    }
}
