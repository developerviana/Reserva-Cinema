using ReservaCinema.Domain.Exceptions;

namespace ReservaCinema.Domain.Entities;

/// <summary>
/// Entidade que representa uma reserva de assentos em uma sessão.
/// </summary>
public class Reservation
{
    /// <summary>
    /// ID único da reserva.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// ID da sessão reservada.
    /// </summary>
    public Guid SessionId { get; set; }

    /// <summary>
    /// ID do usuário que fez a reserva.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Lista de assentos reservados (separados por vírgula).
    /// </summary>
    public string SeatsJson { get; set; } = string.Empty;

    /// <summary>
    /// Status da reserva: pending, confirmed, cancelled.
    /// </summary>
    public string Status { get; set; } = "pending";

    /// <summary>
    /// Data e hora de expiração da reserva.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Valor total da reserva.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// ID da venda (gerado ao confirmar pagamento).
    /// </summary>
    public string? SaleId { get; set; }

    /// <summary>
    /// Data e hora do pagamento confirmado.
    /// </summary>
    public DateTime? PaidAt { get; set; }

    /// <summary>
    /// Data de criação da reserva.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data de atualização da reserva.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Getter para desserializar os assentos do JSON.
    /// </summary>
    public string[] GetSeats()
    {
        if (string.IsNullOrWhiteSpace(SeatsJson))
            return Array.Empty<string>();

        return SeatsJson.Split(',', System.StringSplitOptions.TrimEntries);
    }

    /// <summary>
    /// Setter para serializar os assentos para JSON.
    /// </summary>
    public void SetSeats(string[] seats)
    {
        SeatsJson = seats.Length > 0 ? string.Join(",", seats) : string.Empty;
    }

    /// <summary>
    /// Confirma o pagamento da reserva.
    /// Regras: reserva não pode estar expirada e não pode estar já confirmada.
    /// </summary>
    public void ConfirmPayment(string transactionId)
    {
        if (DateTime.UtcNow > ExpiresAt)
            throw new ReservationExpiredException(ExpiresAt);

        if (Status == "confirmed")
            throw new InvalidOperationException("Reserva já foi confirmada.");

        Status = "confirmed";
        SaleId = GenerateSaleId();
        PaidAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    private static string GenerateSaleId()
    {
        return $"sale-{Guid.NewGuid().ToString()[..8]}";
    }
}
