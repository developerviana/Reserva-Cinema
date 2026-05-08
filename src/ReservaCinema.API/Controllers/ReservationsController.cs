using Microsoft.AspNetCore.Mvc;
using ReservaCinema.Application.DTOs.Reservations;
using ReservaCinema.Application.Services.Interfaces;
using ReservaCinema.Domain.Exceptions;
using ReservaCinema.Application.Validators.Reservations;

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
    private readonly ConfirmPaymentRequestValidator _confirmValidator = new();

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
    /// <response code="400">Dados inválidos.</response>
    /// <response code="409">Assentos já estão reservados.</response>
    [HttpPost]
    [ProducesResponseType(typeof(CreateReservationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateReservation([FromBody] CreateReservationRequest request)
    {
        try
        {
            var reservation = await _reservationService.CreateReservationAsync(request);
            return CreatedAtAction(nameof(CreateReservation), new { id = reservation.ReservationId }, reservation);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (ConflictException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Confirma o pagamento de uma reserva pendente.
    /// </summary>
    /// <param name="id">ID da reserva.</param>
    /// <param name="request">Dados do pagamento.</param>
    /// <response code="200">Pagamento confirmado com sucesso.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="404">Reserva não encontrada.</response>
    /// <response code="410">Reserva expirada.</response>
    [HttpPost("{id}/confirm")]
    [ProducesResponseType(typeof(ConfirmPaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    public async Task<IActionResult> ConfirmPayment(string id, [FromBody] ConfirmPaymentRequest request)
    {
        var validation = await _confirmValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        try
        {
            var response = await _reservationService.ConfirmPaymentAsync(id, request);
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ReservationExpiredException ex)
        {
            return StatusCode(StatusCodes.Status410Gone, new
            {
                error = "RESERVATION_EXPIRED",
                message = ex.Message
            });
        }
    }
}
