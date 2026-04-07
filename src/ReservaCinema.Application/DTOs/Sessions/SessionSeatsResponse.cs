namespace ReservaCinema.Application.DTOs.Sessions;

/// <summary>
/// Response para disponibilidade de assentos de uma sessão.
/// </summary>
public class SessionSeatsResponse
{
    /// <summary>
    /// ID da sessão.
    /// </summary>
    public Guid SessionId { get; set; }

    /// <summary>
    /// Total de assentos da sessão.
    /// </summary>
    public int TotalSeats { get; set; }

    /// <summary>
    /// Quantidade de assentos disponíveis no momento.
    /// </summary>
    public int AvailableSeats { get; set; }

    /// <summary>
    /// Lista detalhada de cada assento e seu status.
    /// </summary>
    public SeatDto[] Seats { get; set; } = Array.Empty<SeatDto>();
}
