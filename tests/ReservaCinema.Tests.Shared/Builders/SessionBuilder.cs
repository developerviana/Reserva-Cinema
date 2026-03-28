using ReservaCinema.Domain.Entities;

namespace ReservaCinema.Tests.Shared.Builders;

/// <summary>
/// Builder para construir instâncias de Session em testes.
/// </summary>
public class SessionBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _movieTitle = "Default Movie";
    private DateTime _startTime = DateTime.UtcNow.AddHours(2);
    private string _roomNumber = "A1";
    private int _totalSeats = 100;
    private int _availableSeats = 100;
    private decimal _ticketPrice = 25.50m;
    private DateTime _createdAt = DateTime.UtcNow;
    private DateTime? _updatedAt = null;
    private bool _isActive = true;
    private int? _durationMinutes = 120;

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

    public SessionBuilder WithAvailableSeats(int availableSeats)
    {
        _availableSeats = availableSeats;
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

    public SessionBuilder WithUpdatedAt(DateTime? updatedAt)
    {
        _updatedAt = updatedAt;
        return this;
    }

    public SessionBuilder WithIsActive(bool isActive)
    {
        _isActive = isActive;
        return this;
    }

    public SessionBuilder WithDurationMinutes(int? durationMinutes)
    {
        _durationMinutes = durationMinutes;
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
            AvailableSeats = _availableSeats,
            TicketPrice = _ticketPrice,
            CreatedAt = _createdAt,
            UpdatedAt = _updatedAt,
            IsActive = _isActive,
            DurationMinutes = _durationMinutes
        };
    }
}
