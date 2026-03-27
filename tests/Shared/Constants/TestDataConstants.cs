using ReservaCinema.Application.DTOs.Sessions;

namespace ReservaCinema.Tests.Shared.Constants;

/// <summary>
/// Constantes para dados de teste.
/// </summary>
public static class TestDataConstants
{
    public static class Sessions
    {
        public const string ValidMovieTitle = "The Matrix";
        public const string ValidRoomNumber = "A1";
        public const int ValidTotalSeats = 100;
        public const decimal ValidTicketPrice = 25.50m;
        
        public const string InvalidMovieTitle = "";
        public const string InvalidRoomNumber = "A";
        public const int InvalidTotalSeats = 0;
        public const decimal InvalidTicketPrice = -10m;

        public static readonly DateTime ValidStartTime = DateTime.UtcNow.AddHours(1);
        public static readonly DateTime InvalidStartTimePast = DateTime.UtcNow.AddHours(-1);
    }

    public static class Validation
    {
        public const string MovieTitleRequired = "Movie title is required";
        public const string RoomNumberRequired = "Room number is required";
        public const string RoomNumberMinLength = "Room number must have at least 2 characters";
        public const string TotalSeatsGreaterThanZero = "Total seats must be greater than 0";
        public const string TicketPriceGreaterThanZero = "Ticket price must be greater than 0";
        public const string StartTimeCannotBeInPast = "Start time cannot be in the past";
    }
}
