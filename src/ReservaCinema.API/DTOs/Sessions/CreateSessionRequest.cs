namespace ReservaCinema.API.DTOs.Sessions;

public class CreateSessionRequest
{
    public string MovieTitle { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int TotalSeats { get; set; }
    public double TicketPrice { get; set; }
}
