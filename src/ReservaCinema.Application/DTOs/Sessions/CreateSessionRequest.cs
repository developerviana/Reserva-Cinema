namespace ReservaCinema.Application.DTOs.Sessions;

/// <summary>
/// Request para criar uma nova sessão de cinema.
/// </summary>
public class CreateSessionRequest
{
    /// <summary>
    /// Título do filme.
    /// </summary>
    public string MovieTitle { get; set; } = string.Empty;

    /// <summary>
    /// Data e horário de início da sessão.
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Número ou identificador da sala.
    /// </summary>
    public string RoomNumber { get; set; } = string.Empty;

    /// <summary>
    /// Total de assentos disponíveis na sala.
    /// </summary>
    public int TotalSeats { get; set; }

    /// <summary>
    /// Preço do ingresso em reais.
    /// </summary>
    public decimal TicketPrice { get; set; }
}
