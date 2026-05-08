using Microsoft.EntityFrameworkCore;
using ReservaCinema.Application.Persistence.Repositories;
using ReservaCinema.Domain.Entities;

namespace ReservaCinema.Infrastructure.Persistence.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly ReservaCinemaDbContext _context;

    public ReservationRepository(ReservaCinemaDbContext context) => _context = context;

    public async Task<Reservation> AddAsync(Reservation reservation)
    {
        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();
        return reservation;
    }

    public async Task<Reservation?> GetByIdAsync(string id) =>
        await _context.Reservations.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);

    public async Task<IEnumerable<Reservation>> GetBySessionIdAsync(Guid sessionId) =>
        await _context.Reservations
            .AsNoTracking()
            .Where(r => r.SessionId == sessionId && r.Status != "cancelled")
            .ToListAsync();

    public async Task<Reservation> UpdateAsync(Reservation reservation)
    {
        _context.Reservations.Update(reservation);
        await _context.SaveChangesAsync();
        return reservation;
    }
}
