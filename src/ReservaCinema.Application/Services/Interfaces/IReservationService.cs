using ReservaCinema.Application.DTOs.Reservations;
using ReservaCinema.Domain.Exceptions;

namespace ReservaCinema.Application.Services.Interfaces;

/// <summary>
/// Interface para serviço de gerenciamento de reservas.
/// </summary>
public interface IReservationService
{
    /// <summary>
    /// Cria uma nova reserva.
    /// </summary>
    Task<CreateReservationResponse> CreateReservationAsync(CreateReservationRequest request);

    /// <summary>
    /// Confirma o pagamento de uma reserva pendente.
    /// </summary>
    /// <exception cref="KeyNotFoundException">Reserva não encontrada.</exception>
    /// <exception cref="ReservationExpiredException">Reserva expirada.</exception>
    Task<ConfirmPaymentResponse> ConfirmPaymentAsync(string reservationId, ConfirmPaymentRequest request);
}
