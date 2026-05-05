using ReservaCinema.Domain.Entities;

namespace ReservaCinema.Application.Persistence.Repositories;

public interface IReservationRepository
{
    Task<Reservation> AddAsync(Reservation reservation);
    Task<Reservation?> GetByIdAsync(string id);
    Task<IEnumerable<Reservation>> GetBySessionIdAsync(Guid sessionId);
}
