using ReservaCinema.Application.DTOs.Reservations;
using ReservaCinema.Application.Persistence.Repositories;
using ReservaCinema.Application.Services.Interfaces;
using ReservaCinema.Domain.Entities;
using ReservaCinema.Domain.Exceptions;

namespace ReservaCinema.Application.Services;

public class ReservationService : IReservationService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IDistributedLockService _lockService;

    // Regra de negócio: processamento de reserva deve concluir em até 30 segundos
    private static readonly TimeSpan ReservationLockExpiration = TimeSpan.FromSeconds(30);

    public ReservationService(
        ISessionRepository sessionRepository,
        IReservationRepository reservationRepository,
        IDistributedLockService lockService)
    {
        _sessionRepository = sessionRepository;
        _reservationRepository = reservationRepository;
        _lockService = lockService;
    }

    public async Task<CreateReservationResponse> CreateReservationAsync(CreateReservationRequest request)
    {
        var session = await _sessionRepository.GetByIdAsync(request.SessionId)
            ?? throw new KeyNotFoundException($"Sessão {request.SessionId} não encontrada.");

        if (session.AvailableSeats < request.SeatNumbers.Length)
            throw new ConflictException("Assentos insuficientes disponíveis para esta sessão.");

        var lockKey = $"reservation:session:{request.SessionId}:seats:{string.Join(":", request.SeatNumbers.OrderBy(s => s))}";
        var lockToken = await _lockService.AcquireLockAsync(lockKey, ReservationLockExpiration);

        if (lockToken == null)
            throw new ConflictException("Não foi possível adquirir lock para reserva. Tente novamente.");

        try
        {
            var reservation = new Reservation
            {
                Id = Guid.NewGuid().ToString(),
                SessionId = request.SessionId,
                UserId = request.UserId,
                Status = "pending",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                TotalAmount = request.SeatNumbers.Length * session.TicketPrice,
            };
            reservation.SetSeats(request.SeatNumbers);

            await _reservationRepository.AddAsync(reservation);

            return new CreateReservationResponse
            {
                ReservationId = reservation.Id,
                Status = reservation.Status,
                ExpiresAt = reservation.ExpiresAt,
                Seats = request.SeatNumbers,
                TotalAmount = reservation.TotalAmount
            };
        }
        finally
        {
            await _lockService.ReleaseLockAsync(lockKey, lockToken);
        }
    }

    public async Task<ConfirmPaymentResponse> ConfirmPaymentAsync(string reservationId, ConfirmPaymentRequest request)
    {
        var reservation = await _reservationRepository.GetByIdAsync(reservationId)
            ?? throw new KeyNotFoundException($"Reserva {reservationId} não encontrada.");

        if (DateTime.UtcNow > reservation.ExpiresAt)
            throw new ReservationExpiredException(reservation.ExpiresAt);

        var paidAt = DateTime.UtcNow;

        reservation.Status = "confirmed";
        reservation.SaleId = $"sale-{Guid.NewGuid()}";
        reservation.PaymentMethod = request.PaymentMethod;
        reservation.TransactionId = request.TransactionId;
        reservation.PaidAt = paidAt;
        reservation.UpdatedAt = paidAt;

        await _reservationRepository.UpdateAsync(reservation);

        return new ConfirmPaymentResponse
        {
            SaleId = reservation.SaleId,
            Status = reservation.Status,
            Seats = reservation.GetSeats(),
            PaidAt = paidAt
        };
    }
}
