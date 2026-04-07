namespace ReservaCinema.Application.DTOs.Sessions;

/// <summary>
/// Status possível de um assento.
/// </summary>
public enum SeatStatus
{
    /// <summary>
    /// Assento disponível para compra.
    /// </summary>
    Available,

    /// <summary>
    /// Assento reservado (não pago, com expiração).
    /// </summary>
    Reserved,

    /// <summary>
    /// Assento vendido (pago e confirmado).
    /// </summary>
    Sold
}

/// <summary>
/// DTO para um assento individual.
/// </summary>
public class SeatDto
{
    /// <summary>
    /// Número/identificador do assento (ex: A1, B2).
    /// </summary>
    public string Number { get; set; } = string.Empty;

    /// <summary>
    /// Status atual do assento.
    /// </summary>
    public SeatStatus Status { get; set; }

    /// <summary>
    /// Data de expiração da reserva (apenas se Status = Reserved).
    /// </summary>
    public DateTime? ExpiresAt { get; set; }
}
