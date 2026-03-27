using ReservaCinema.Application.DTOs.Sessions;

namespace ReservaCinema.Tests.Shared.Builders;

/// <summary>
/// Builder para criar CreateSessionRequest em testes.
/// </summary>
public class CreateSessionRequestBuilder
{
    private string _movieTitle = "Test Movie";
    private DateTime _startTime = DateTime.UtcNow.AddHours(1);
    private string _roomNumber = "A1";
    private int _totalSeats = 100;
    private decimal _ticketPrice = 25.50m;

    public CreateSessionRequestBuilder WithMovieTitle(string movieTitle)
    {
        _movieTitle = movieTitle;
        return this;
    }

    public CreateSessionRequestBuilder WithStartTime(DateTime startTime)
    {
        _startTime = startTime;
        return this;
    }

    public CreateSessionRequestBuilder WithRoomNumber(string roomNumber)
    {
        _roomNumber = roomNumber;
        return this;
    }

    public CreateSessionRequestBuilder WithTotalSeats(int totalSeats)
    {
        _totalSeats = totalSeats;
        return this;
    }

    public CreateSessionRequestBuilder WithTicketPrice(decimal ticketPrice)
    {
        _ticketPrice = ticketPrice;
        return this;
    }

    public CreateSessionRequest Build()
    {
        return new CreateSessionRequest
        {
            MovieTitle = _movieTitle,
            StartTime = _startTime,
            RoomNumber = _roomNumber,
            TotalSeats = _totalSeats,
            TicketPrice = _ticketPrice
        };
    }
}
