using ReservaCinema.Application.DTOs.Reservations;
using ReservaCinema.Application.Services.Interfaces;
using ReservaCinema.Domain.Entities;

namespace ReservaCinema.Application.Services;

/// <summary>
/// Serviço para gerenciamento de reservas de assentos.
/// </summary>
public class ReservationService : IReservationService
{
    public async Task<CreateReservationResponse> CreateReservationAsync(CreateReservationRequest request)
    {
        // Validação de entrada
        if (request.SessionId == Guid.Empty)
            throw new ArgumentException("SessionId inválido.");

        if (string.IsNullOrWhiteSpace(request.UserId))
            throw new ArgumentException("UserId não pode estar vazio.");

        if (request.SeatNumbers == null || request.SeatNumbers.Length == 0)
            throw new ArgumentException("Deve haver pelo menos um assento a ser reservado.");

        // Validação de limite de assentos
        if (request.SeatNumbers.Length > 10)
            throw new ArgumentException("Máximo de 10 assentos por reserva.");

        // Validação de assentos duplicados
        var uniqueSeats = request.SeatNumbers.Distinct();
        if (uniqueSeats.Count() != request.SeatNumbers.Length)
            throw new ArgumentException("Não pode haver assentos duplicados na reserva.");

        // Simula criação de reserva (em produção, integraria com banco de dados)
        var reservationId = $"res-{Guid.NewGuid().ToString().Substring(0, 8)}";
        var expiresAt = DateTime.UtcNow.AddHours(1);
        var totalAmount = request.SeatNumbers.Length * 25.50m; // Valor base por assento

        var response = new CreateReservationResponse
        {
            ReservationId = reservationId,
            Status = "pending",
            ExpiresAt = expiresAt,
            Seats = request.SeatNumbers,
            TotalAmount = totalAmount
        };

        return await Task.FromResult(response);
    }
}
