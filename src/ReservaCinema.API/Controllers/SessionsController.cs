using Microsoft.AspNetCore.Mvc;
using ReservaCinema.Application.DTOs.Sessions;
using ReservaCinema.Application.Validators.Sessions;

namespace ReservaCinema.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionsController : ControllerBase
{
    private readonly CreateSessionRequestValidator _validator = new();

    [HttpPost]
    public IActionResult CreateSession([FromBody] CreateSessionRequest request)
    {
        var validationResult = _validator.Validate(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(new { errors = validationResult.Errors });
        }

        return CreatedAtAction(nameof(CreateSession), new { message = "Session created successfully", request });
    }
}
