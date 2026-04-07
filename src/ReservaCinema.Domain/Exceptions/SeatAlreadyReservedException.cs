namespace ReservaCinema.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando um ou mais assentos já estão reservados.
/// </summary>
public class SeatAlreadyReservedException : Exception
{
    public SeatAlreadyReservedException(IEnumerable<string> conflictingSeats)
        : base($"Assentos {string.Join(", ", conflictingSeats)} já estão reservados.")
    {
        ConflictingSeats = conflictingSeats.ToList();
    }

    public SeatAlreadyReservedException(string message, IEnumerable<string> conflictingSeats)
        : base(message)
    {
        ConflictingSeats = conflictingSeats.ToList();
    }

    public IList<string> ConflictingSeats { get; }
}
