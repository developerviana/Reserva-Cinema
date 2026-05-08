namespace ReservaCinema.Domain.Exceptions;

public class ReservationExpiredException : Exception
{
    public DateTime ExpiredAt { get; }

    public ReservationExpiredException(DateTime expiredAt)
        : base($"Reserva expirou em {expiredAt:O}")
    {
        ExpiredAt = expiredAt;
    }
}
