namespace ReservaCinema.Application.Entities;

public class Session
{
    public int Id { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int TotalSeats { get; set; }
    public double TicketPrice { get; set; }
    public int AvailableSeats { get; set; }
    public DateTime CreatedAt { get; set; }
}
