using ReservaCinema.Application.DTOs.Reservations;

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
}
