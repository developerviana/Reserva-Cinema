namespace ReservaCinema.Application.DTOs.Reservations;

/// <summary>
/// Response de erro quando há conflito de assentos.
/// </summary>
public class ReservationConflictResponse
{
    /// <summary>
    /// Código do erro.
    /// </summary>
    public string Error { get; set; } = string.Empty;

    /// <summary>
    /// Mensagem descritiva do erro.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Lista de assentos que causaram conflito.
    /// </summary>
    public string[] ConflictingSeats { get; set; } = Array.Empty<string>();
}
