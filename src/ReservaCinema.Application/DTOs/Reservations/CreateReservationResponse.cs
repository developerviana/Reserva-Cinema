namespace ReservaCinema.Application.DTOs.Reservations;

/// <summary>
/// Response ao criar uma reserva com sucesso.
/// </summary>
public class CreateReservationResponse
{
    /// <summary>
    /// ID único da reserva.
    /// </summary>
    public string ReservationId { get; set; } = string.Empty;

    /// <summary>
    /// Status da reserva (pending, confirmed, cancelled).
    /// </summary>
    public string Status { get; set; } = "pending";

    /// <summary>
    /// Data e hora de expiração da reserva.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Assentos reservados.
    /// </summary>
    public string[] Seats { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Valor total da reserva.
    /// </summary>
    public decimal TotalAmount { get; set; }
}
