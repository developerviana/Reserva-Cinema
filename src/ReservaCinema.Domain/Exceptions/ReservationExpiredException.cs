namespace ReservaCinema.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando uma reserva expirou.
/// </summary>
public class ReservationExpiredException : Exception
{
    public ReservationExpiredException(DateTime expiresAt)
        : base($"Reserva expirou em {expiresAt:O}")
    {
        ExpiresAt = expiresAt;
    }

    public DateTime ExpiresAt { get; }
}
