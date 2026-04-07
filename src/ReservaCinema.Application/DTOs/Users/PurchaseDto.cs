namespace ReservaCinema.Application.DTOs.Users;

/// <summary>
/// DTO para uma compra individual do usuário.
/// </summary>
public class PurchaseDto
{
    /// <summary>
    /// ID único da venda/transação.
    /// </summary>
    public string SaleId { get; set; } = string.Empty;

    /// <summary>
    /// ID da sessão de cinema.
    /// </summary>
    public Guid SessionId { get; set; }

    /// <summary>
    /// Título do filme.
    /// </summary>
    public string MovieTitle { get; set; } = string.Empty;

    /// <summary>
    /// Assentos comprados.
    /// </summary>
    public string[] Seats { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Valor total da compra.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Data e hora da compra.
    /// </summary>
    public DateTime PurchasedAt { get; set; }
}
