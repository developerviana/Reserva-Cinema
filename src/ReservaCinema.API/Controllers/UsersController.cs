using Microsoft.AspNetCore.Mvc;
using ReservaCinema.Application.DTOs.Users;
using ReservaCinema.Application.Services.Interfaces;

namespace ReservaCinema.API.Controllers;

/// <summary>
/// Controller para gerenciamento de usuários e seu histórico de compras.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public UsersController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    /// <summary>
    /// Obtém o histórico de compras de um usuário.
    /// </summary>
    /// <param name="userId">ID do usuário.</param>
    /// <returns>Retorna o histórico de compras do usuário.</returns>
    /// <remarks>
    /// Exemplo de requisição:
    ///
    ///     GET /api/users/user-123/purchases
    ///
    /// Retorna um objeto com userId e um array de purchases contendo:
    /// - saleId: ID da transação
    /// - sessionId: ID da sessão
    /// - movieTitle: Título do filme
    /// - seats: Assentos comprados
    /// - totalAmount: Valor total pago
    /// - purchasedAt: Data/hora da compra
    ///
    /// </remarks>
    /// <response code="200">Histórico de compras retornado com sucesso (pode estar vazio).</response>
    /// <response code="400">Dados inválidos (userId vazio ou null).</response>
    [HttpGet("{userId}/purchases")]
    [ProducesResponseType(typeof(PurchaseHistoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPurchaseHistory(string userId)
    {
        try
        {
            var result = await _reservationService.GetPurchaseHistoryAsync(userId);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = "INVALID_REQUEST", message = ex.Message });
        }
    }
}
