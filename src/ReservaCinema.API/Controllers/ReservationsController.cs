using Microsoft.AspNetCore.Mvc;
using ReservaCinema.Application.DTOs.Reservations;
using ReservaCinema.Application.Services.Interfaces;

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
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
