using ReservaCinema.Domain.Entities;

namespace ReservaCinema.Tests.Shared.Builders;

/// <summary>
/// Builder para criar entidades Session em testes.
/// </summary>
public class SessionBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _movieTitle = "Test Movie";
    private DateTime _startTime = DateTime.UtcNow.AddHours(1);
    private string _roomNumber = "A1";
    private int _totalSeats = 100;
    private decimal _ticketPrice = 25.50m;
    private DateTime _createdAt = DateTime.UtcNow;

    public SessionBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public SessionBuilder WithMovieTitle(string movieTitle)
    {
        _movieTitle = movieTitle;
        return this;
    }

    public SessionBuilder WithStartTime(DateTime startTime)
    {
        _startTime = startTime;
        return this;
    }

    public SessionBuilder WithRoomNumber(string roomNumber)
    {
        _roomNumber = roomNumber;
        return this;
    }

    public SessionBuilder WithTotalSeats(int totalSeats)
    {
        _totalSeats = totalSeats;
        return this;
    }

    public SessionBuilder WithTicketPrice(decimal ticketPrice)
    {
        _ticketPrice = ticketPrice;
        return this;
    }

    public SessionBuilder WithCreatedAt(DateTime createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    public Session Build()
    {
        return new Session
        {
            Id = _id,
            MovieTitle = _movieTitle,
            StartTime = _startTime,
            RoomNumber = _roomNumber,
            TotalSeats = _totalSeats,
            TicketPrice = _ticketPrice,
            CreatedAt = _createdAt
        };
    }
}
