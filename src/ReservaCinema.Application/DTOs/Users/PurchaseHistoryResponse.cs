namespace ReservaCinema.Application.DTOs.Users;

/// <summary>
/// Response para o histórico de compras do usuário.
/// </summary>
public class PurchaseHistoryResponse
{
    /// <summary>
    /// ID do usuário.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Lista de compras do usuário.
    /// </summary>
    public PurchaseDto[] Purchases { get; set; } = Array.Empty<PurchaseDto>();
}
