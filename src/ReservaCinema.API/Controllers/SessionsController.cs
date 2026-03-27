using Microsoft.AspNetCore.Mvc;
using ReservaCinema.API.DTOs.Sessions;
using ReservaCinema.API.Validators.Sessions;

namespace ReservaCinema.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionsController : ControllerBase
{
    private readonly CreateSessionRequestValidator _validator = new();

    [HttpPost]
    public IActionResult CreateSession([FromBody] CreateSessionRequest request)
    {
        // Validar
        var validationResult = _validator.Validate(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(new { errors = validationResult.Errors });
        }

        // TODO: Implementar lógica de criação de sessão
        return CreatedAtAction(nameof(CreateSession), new { message = "Session created successfully", request });
    }
}
