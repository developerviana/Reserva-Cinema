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
}
