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
    /// Lista de assentos reservados, armazenada como CSV (ex: "A1,A2,B3").
    /// </summary>
    public string Seats { get; set; } = string.Empty;

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
        if (string.IsNullOrWhiteSpace(Seats))
            return Array.Empty<string>();

        return Seats.Split(',', System.StringSplitOptions.TrimEntries);
    }

    /// <summary>
    /// Setter para serializar os assentos para JSON.
    /// </summary>
    public void SetSeats(string[] seats)
    {
        Seats = seats.Length > 0 ? string.Join(",", seats) : string.Empty;
    }
}
