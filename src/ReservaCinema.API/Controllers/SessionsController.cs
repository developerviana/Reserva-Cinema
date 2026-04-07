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

    /// <summary>
    /// Obtém uma sessão de cinema pelo ID.
    /// </summary>
    /// <param name="id">ID da sessão a ser recuperada.</param>
    /// <returns>Retorna a sessão encontrada com status 200.</returns>
    /// <response code="200">Sessão encontrada com sucesso.</response>
    /// <response code="404">Sessão não encontrada.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSessionById(Guid id)
    {
        var session = await _sessionService.GetSessionByIdAsync(id);

        if (session is null)
        {
            return NotFound(new { error = "Sessão não encontrada." });
        }

        return Ok(session);
    }

    /// <summary>
    /// Obtém a disponibilidade em tempo real de assentos de uma sessão.
    /// </summary>
    /// <param name="id">ID da sessão.</param>
    /// <returns>Retorna informações de assentos disponíveis, reservados e vendidos.</returns>
    /// <remarks>
    /// Exemplo de resposta:
    ///
    ///     GET /api/sessions/550e8400-e29b-41d4-a716-446655440000/seats
    ///     {
    ///        "sessionId": "550e8400-e29b-41d4-a716-446655440000",
    ///        "totalSeats": 16,
    ///        "availableSeats": 8,
    ///        "seats": [
    ///          { "number": "A1", "status": "available" },
    ///          { "number": "A2", "status": "reserved", "expiresAt": "2024-02-15T19:00:30Z" },
    ///          { "number": "A3", "status": "sold" }
    ///        ]
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Assentos encontrados com sucesso.</response>
    /// <response code="404">Sessão não encontrada.</response>
    [HttpGet("{id:guid}/seats")]
    [ProducesResponseType(typeof(SessionSeatsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSessionSeats(Guid id)
    {
        var seatsInfo = await _sessionService.GetSessionSeatsAsync(id);

        if (seatsInfo is null)
        {
            return NotFound(new { error = "Sessão não encontrada." });
        }

        return Ok(seatsInfo);
    }
}
