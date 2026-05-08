namespace ReservaCinema.Application.DTOs.Reservations;

/// <summary>
/// Request para confirmar o pagamento de uma reserva.
/// </summary>
public class ConfirmPaymentRequest
{
    /// <summary>
    /// Método de pagamento utilizado (ex: credit_card).
    /// </summary>
    public string PaymentMethod { get; set; } = string.Empty;

    /// <summary>
    /// ID da transação fornecido pelo gateway de pagamento.
    /// </summary>
    public string TransactionId { get; set; } = string.Empty;
}
