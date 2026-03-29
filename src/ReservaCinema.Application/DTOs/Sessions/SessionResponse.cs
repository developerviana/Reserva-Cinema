namespace ReservaCinema.Application.DTOs.Sessions;

/// <summary>
/// Response ao criar ou recuperar uma sessão.
/// </summary>
public class SessionResponse
{
    /// <summary>
    /// ID único da sessão.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Título do filme.
    /// </summary>
    public string MovieTitle { get; set; } = string.Empty;

    /// <summary>
    /// Data e horário da sessão.
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Número da sala.
    /// </summary>
    public string RoomNumber { get; set; } = string.Empty;

    /// <summary>
    /// Total de assentos da sala.
    /// </summary>
    public int TotalSeats { get; set; }

    /// <summary>
    /// Assentos disponíveis.
    /// </summary>
    public int AvailableSeats { get; set; }

    /// <summary>
    /// Preço do ingresso.
    /// </summary>
    public decimal TicketPrice { get; set; }

    /// <summary>
    /// Data de criação da sessão.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Data de última atualização da sessão.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indica se a sessão está ativa.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Duração do filme em minutos.
    /// </summary>
    public int? DurationMinutes { get; set; }

    /// <summary>
    /// Classificação indicativa (L, 10, 12, 14, 16, 18).
    /// </summary>
    public string? RatingClassification { get; set; }
}
