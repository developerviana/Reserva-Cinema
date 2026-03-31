namespace ReservaCinema.Application.DTOs.Reservations;

/// <summary>
/// Request para criar uma nova reserva.
/// </summary>
public class CreateReservationRequest
{
    /// <summary>
    /// ID da sessão a ser reservada.
    /// </summary>
    public Guid SessionId { get; set; }

    /// <summary>
    /// ID do usuário fazendo a reserva.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Lista de assentos a serem reservados.
    /// </summary>
    public string[] SeatNumbers { get; set; } = Array.Empty<string>();
}
