namespace ReservaCinema.Application.DTOs.Sessions;

public class CreateSessionRequest
{
    public string? MovieTitle { get; set; }
    public DateTime StartTime { get; set; }
    public string? RoomNumber { get; set; }
    public int TotalSeats { get; set; }
    public double TicketPrice { get; set; }
}
