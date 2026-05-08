namespace ReservaCinema.Application.DTOs.Reservations;

/// <summary>
/// Response ao confirmar o pagamento de uma reserva.
/// </summary>
public class ConfirmPaymentResponse
{
    /// <summary>
    /// ID da venda gerada.
    /// </summary>
    public string SaleId { get; set; } = string.Empty;

    /// <summary>
    /// Status da reserva após confirmação.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Assentos confirmados.
    /// </summary>
    public string[] Seats { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Data e hora em que o pagamento foi confirmado.
    /// </summary>
    public DateTime PaidAt { get; set; }
}
