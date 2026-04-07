using ReservaCinema.Application.DTOs.Reservations;
using ReservaCinema.Application.DTOs.Users;

namespace ReservaCinema.Application.Services.Interfaces;

/// <summary>
/// Interface para serviço de gerenciamento de reservas.
/// </summary>
public interface IReservationService
{
    /// <summary>
    /// Cria uma nova reserva.
    /// </summary>
    /// <param name="request">Dados da reserva a ser criada.</param>
    /// <returns>Response com dados da reserva criada.</returns>
    /// <exception cref="ArgumentException">Lançada se dados inválidos ou assentos indisponíveis.</exception>
    Task<CreateReservationResponse> CreateReservationAsync(CreateReservationRequest request);

    /// <summary>
    /// Confirma o pagamento de uma reserva existente.
    /// </summary>
    /// <param name="reservationId">ID da reserva a confirmar.</param>
    /// <param name="request">Dados de pagamento.</param>
    /// <returns>Response com dados da confirmação ou null se reserva não existir.</returns>
    /// <exception cref="ArgumentException">Lançada se dados inválidos.</exception>
    /// <exception cref="ReservaCinema.Domain.Exceptions.ReservationExpiredException">Lançada se reserva expirou.</exception>
    Task<ConfirmPaymentResponse?> ConfirmPaymentAsync(string reservationId, ConfirmPaymentRequest request);

    /// <summary>
    /// Obtém o histórico de compras de um usuário.
    /// </summary>
    /// <param name="userId">ID do usuário.</param>
    /// <returns>Response com lista de compras do usuário.</returns>
    /// <exception cref="ArgumentException">Lançada se userId inválido.</exception>
    Task<PurchaseHistoryResponse> GetPurchaseHistoryAsync(string userId);
}
