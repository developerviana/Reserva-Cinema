using ReservaCinema.Domain.Entities;

namespace ReservaCinema.Tests.Shared.Builders;

public class ReservationBuilder
{
    private string _id = Guid.NewGuid().ToString();
    private Guid _sessionId = Guid.NewGuid();
    private string _userId = "user-test";
    private string[] _seats = ["A1", "A2"];
    private string _status = "pending";
    private DateTime _expiresAt = DateTime.UtcNow.AddHours(1);
    private decimal _totalAmount = 51.00m;

    public ReservationBuilder WithId(string id) { _id = id; return this; }
    public ReservationBuilder WithSessionId(Guid sessionId) { _sessionId = sessionId; return this; }
    public ReservationBuilder WithUserId(string userId) { _userId = userId; return this; }
    public ReservationBuilder WithSeats(params string[] seats) { _seats = seats; return this; }
    public ReservationBuilder WithStatus(string status) { _status = status; return this; }
    public ReservationBuilder WithTotalAmount(decimal totalAmount) { _totalAmount = totalAmount; return this; }
    public ReservationBuilder WithExpiresAt(DateTime expiresAt) { _expiresAt = expiresAt; return this; }

    public Reservation Build()
    {
        var reservation = new Reservation
        {
            Id = _id,
            SessionId = _sessionId,
            UserId = _userId,
            Status = _status,
            ExpiresAt = _expiresAt,
            TotalAmount = _totalAmount,
        };
        reservation.SetSeats(_seats);
        return reservation;
    }
}
