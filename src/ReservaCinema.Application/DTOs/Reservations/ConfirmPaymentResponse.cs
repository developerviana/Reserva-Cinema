namespace ReservaCinema.Application.DTOs.Reservations;

/// <summary>
/// Response ao confirmar o pagamento de uma reserva com sucesso.
/// </summary>
public class ConfirmPaymentResponse
{
    /// <summary>
    /// ID único da venda/transação.
    /// </summary>
    public string SaleId { get; set; } = string.Empty;

    /// <summary>
    /// Status da reserva após confirmação (confirmed).
    /// </summary>
    public string Status { get; set; } = "confirmed";

    /// <summary>
    /// Assentos confirmados.
    /// </summary>
    public string[] Seats { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Data e hora da confirmação do pagamento.
    /// </summary>
    public DateTime PaidAt { get; set; }
}
