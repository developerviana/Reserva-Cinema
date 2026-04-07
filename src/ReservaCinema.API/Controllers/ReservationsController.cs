using Microsoft.AspNetCore.Mvc;
using ReservaCinema.Application.DTOs.Reservations;
using ReservaCinema.Application.Services.Interfaces;
using ReservaCinema.Domain.Exceptions;

namespace ReservaCinema.API.Controllers;

/// <summary>
/// Controller para gerenciamento de reservas de cinema.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    /// <summary>
    /// Cria uma nova reserva de assentos.
    /// </summary>
    /// <param name="request">Dados da reserva a ser criada.</param>
    /// <returns>Retorna a reserva criada com status 201.</returns>
    /// <remarks>
    /// Exemplo de requisição:
    ///
    ///     POST /api/reservations
    ///     {
    ///        "sessionId": "550e8400-e29b-41d4-a716-446655440000",
    ///        "userId": "user-123",
    ///        "seatNumbers": ["A1", "A2"]
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Reserva criada com sucesso.</response>
    /// <response code="400">Validação falhou - dados inválidos.</response>
    /// <response code="409">Assentos já estão reservados.</response>
    [HttpPost]
    [ProducesResponseType(typeof(CreateReservationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ReservationConflictResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateReservation([FromBody] CreateReservationRequest request)
    {
        try
        {
            var reservation = await _reservationService.CreateReservationAsync(request);
            return CreatedAtAction(nameof(CreateReservation), new { id = reservation.ReservationId }, reservation);
        }
        catch (SeatAlreadyReservedException ex)
        {
            var conflictResponse = new ReservationConflictResponse
            {
                Error = "SEAT_ALREADY_RESERVED",
                Message = ex.Message,
                ConflictingSeats = ex.ConflictingSeats.ToArray()
            };
            return Conflict(conflictResponse);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message }); 
        }
    }

    /// <summary>
    /// Confirma o pagamento de uma reserva existente.
    /// </summary>
    /// <param name="id">ID da reserva a confirmar.</param>
    /// <param name="request">Dados do pagamento (método e ID de transação).</param>
    /// <returns>Retorna confirmação com venda ID e data de pagamento.</returns>
    /// <remarks>
    /// Exemplo de requisição:
    ///
    ///     POST /api/reservations/res-abc12345/confirm
    ///     {
    ///        "paymentMethod": "credit_card",
    ///        "transactionId": "tx-456"
    ///     }
    ///
    /// Respostas:
    /// - 200 OK: Pagamento confirmado com sucesso
    /// - 400 Bad Request: Dados inválidos
    /// - 404 Not Found: Reserva não existe
    /// - 410 Gone: Reserva expirou
    ///
    /// </remarks>
    /// <response code="200">Pagamento confirmado com sucesso.</response>
    /// <response code="400">Validação falhou - dados inválidos.</response>
    /// <response code="404">Reserva não encontrada.</response>
    /// <response code="410">Reserva expirou.</response>
    [HttpPost("{id}/confirm")]
    [ProducesResponseType(typeof(ConfirmPaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status410Gone)]
    public async Task<IActionResult> ConfirmPayment(string id, [FromBody] ConfirmPaymentRequest request)
    {
        try
        {
            var result = await _reservationService.ConfirmPaymentAsync(id, request);

            if (result == null)
                return NotFound(new { error = "RESERVATION_NOT_FOUND", message = "Reserva não encontrada." });

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = "INVALID_REQUEST", message = ex.Message });
        }
        catch (ReservationExpiredException ex)
        {
            return StatusCode(StatusCodes.Status410Gone, new
            {
                error = "RESERVATION_EXPIRED",
                message = $"Reserva expirou em {ex.ExpiresAt:O}"
            });
        }
    }
}
