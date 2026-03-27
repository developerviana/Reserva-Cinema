namespace ReservaCinema.Tests.Shared.Constants;

/// <summary>
/// Constantes para dados de teste.
/// </summary>
public static class TestDataConstants
{
    /// <summary>
    /// Constantes para testes de Session.
    /// </summary>
    public static class Sessions
    {
        // Dados válidos
        public const string ValidMovieTitle = "The Matrix";
        public const string ValidRoomNumber = "A1";
        public const int ValidTotalSeats = 100;
        public const decimal ValidTicketPrice = 25.50m;
        public static readonly DateTime ValidStartTime = DateTime.UtcNow.AddHours(2);

        // Dados inválidos
        public const string InvalidMovieTitle = "";
        public const string InvalidRoomNumber = "A";
        public const int InvalidTotalSeats = 0;
        public const decimal InvalidTicketPrice = -10m;
        public static readonly DateTime InvalidStartTimePast = DateTime.UtcNow.AddHours(-1);
    }
}
