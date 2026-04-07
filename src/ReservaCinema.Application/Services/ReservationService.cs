using ReservaCinema.Application.DTOs.Reservations;
using ReservaCinema.Application.DTOs.Users;
using ReservaCinema.Application.Services.Interfaces;
using ReservaCinema.Domain.Entities;
using ReservaCinema.Domain.Exceptions;

namespace ReservaCinema.Application.Services;

/// <summary>
/// Serviço para gerenciamento de reservas de assentos.
/// </summary>
public class ReservationService : IReservationService
{
    // Simula um repositório em memória para os testes
    private static readonly Dictionary<string, Reservation> ReservationStore = new();

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

        var reservation = new Reservation
        {
            Id = reservationId,
            SessionId = request.SessionId,
            UserId = request.UserId,
            Status = "pending",
            ExpiresAt = expiresAt,
            TotalAmount = totalAmount,
            CreatedAt = DateTime.UtcNow
        };
        reservation.SetSeats(request.SeatNumbers);

        ReservationStore[reservationId] = reservation;

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

    public async Task<ConfirmPaymentResponse?> ConfirmPaymentAsync(string reservationId, ConfirmPaymentRequest request)
    {
        // Validação de entrada
        if (string.IsNullOrWhiteSpace(reservationId))
            throw new ArgumentException("ReservationId não pode estar vazio.");

        if (!reservationId.StartsWith("res-"))
            throw new ArgumentException("ReservationId deve começar com 'res-'.");

        if (string.IsNullOrWhiteSpace(request.PaymentMethod))
            throw new ArgumentException("PaymentMethod não pode estar vazio.");

        if (string.IsNullOrWhiteSpace(request.TransactionId))
            throw new ArgumentException("TransactionId não pode estar vazio.");

        // Busca a reserva (simula repositório)
        if (!ReservationStore.TryGetValue(reservationId, out var reservation))
            return null;

        // Chama o método de domínio que valida as regras de negócio
        reservation.ConfirmPayment(request.TransactionId);

        // Mapeia para response
        var response = new ConfirmPaymentResponse
        {
            SaleId = reservation.SaleId!,
            Status = reservation.Status,
            Seats = reservation.GetSeats(),
            PaidAt = reservation.PaidAt!.Value
        };

        return await Task.FromResult(response);
    }

    public async Task<PurchaseHistoryResponse> GetPurchaseHistoryAsync(string userId)
    {
        // Validação de entrada
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId não pode estar vazio.");

        // Filtra reservas confirmadas do usuário
        var userPurchases = ReservationStore.Values
            .Where(r => r.UserId == userId && r.Status == "confirmed")
            .OrderByDescending(r => r.PaidAt)
            .Select(r => new PurchaseDto
            {
                SaleId = r.SaleId!,
                SessionId = r.SessionId,
                MovieTitle = GetMovieTitle(r.SessionId),
                Seats = r.GetSeats(),
                TotalAmount = r.TotalAmount,
                PurchasedAt = r.PaidAt!.Value
            })
            .ToArray();

        var response = new PurchaseHistoryResponse
        {
            UserId = userId,
            Purchases = userPurchases
        };

        return await Task.FromResult(response);
    }

    /// <summary>
    /// Simula a busca do título do filme pela sessionId.
    /// Em produção, seria feita uma query no banco de dados.
    /// </summary>
    private static string GetMovieTitle(Guid sessionId)
    {
        // Simulação: retorna um título baseado no guid
        return sessionId.ToString().StartsWith("550e8400")
            ? "Oppenheimer"
            : $"Movie-{sessionId.ToString()[..8]}";
    }
}
