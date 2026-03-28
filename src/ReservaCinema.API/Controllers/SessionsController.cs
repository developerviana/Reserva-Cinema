using Microsoft.AspNetCore.Mvc;
using ReservaCinema.Application.DTOs.Sessions;
using ReservaCinema.Application.Services.Interfaces;

namespace ReservaCinema.API.Controllers;

/// <summary>
/// Controller para gerenciamento de sessões de cinema.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SessionsController : ControllerBase
{
    private readonly ISessionService _sessionService;

    public SessionsController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    /// <summary>
    /// Cria uma nova sessão de cinema.
    /// </summary>
    /// <param name="request">Dados da sessão a ser criada.</param>
    /// <returns>Retorna a sessão criada com status 201.</returns>
    /// <remarks>
    /// Exemplo de requisição:
    ///
    ///     POST /api/sessions
    ///     {
    ///        "movieTitle": "The Matrix",
    ///        "startTime": "2026-03-27T19:30:00Z",
    ///        "roomNumber": "A1",
    ///        "totalSeats": 100,
    ///        "ticketPrice": 25.50
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Sessão criada com sucesso.</response>
    /// <response code="400">Validação falhou - dados inválidos.</response>
    [HttpPost]
    [ProducesResponseType(typeof(SessionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSession([FromBody] CreateSessionRequest request)
    {
        try
        {
            var session = await _sessionService.CreateSessionAsync(request);
            return CreatedAtAction(nameof(CreateSession), new { id = session.Id }, session);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
