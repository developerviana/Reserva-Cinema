namespace ReservaCinema.Domain.Entities;

/// <summary>
/// Entidade que representa uma sessão de cinema.
/// </summary>
public class Session
{
    /// <summary>
    /// Construtor padrão para EF Core.
    /// </summary>
    public Session()
    {
    }

    /// <summary>
    /// Construtor para criar uma nova sessão.
    /// </summary>
    public Session(string movieTitle, string roomNumber, DateTime startTime, int totalSeats, decimal ticketPrice)
    {
        MovieTitle = movieTitle;
        StartTime = startTime;
        RoomNumber = roomNumber;
        TotalSeats = totalSeats;
        AvailableSeats = totalSeats;
        TicketPrice = ticketPrice;
    }

    /// <summary>
    /// ID único da sessão.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Título do filme exibido.
    /// </summary>
    public string MovieTitle { get; set; } = string.Empty;

    /// <summary>
    /// Data e horário de início da sessão.
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Número ou identificação da sala.
    /// </summary>
    public string RoomNumber { get; set; } = string.Empty;

    /// <summary>
    /// Total de assentos disponíveis na sala.
    /// </summary>
    public int TotalSeats { get; set; }

    /// <summary>
    /// Quantidade de assentos ainda disponíveis.
    /// </summary>
    public int AvailableSeats { get; set; }

    /// <summary>
    /// Preço do ingresso para essa sessão.
    /// </summary>
    public decimal TicketPrice { get; set; }

    /// <summary>
    /// Data e hora de criação do registro.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data e hora da última atualização.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indica se a sessão está ativa.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Duração do filme em minutos.
    /// </summary>
    public int? DurationMinutes { get; set; }

    /// <summary>
    /// Classificação indicativa (L, 10, 12, 14, 16, 18).
    /// </summary>
    public string? RatingClassification { get; set; }
}
