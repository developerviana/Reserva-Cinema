using Swashbuckle.AspNetCore.Filters;
using ReservaCinema.Application.DTOs.Sessions;

namespace ReservaCinema.Application.Examples.Sessions;

public class CreateSessionRequestExample : IExamplesProvider<CreateSessionRequest>
{
    public CreateSessionRequest GetExamples()
    {
        return new CreateSessionRequest
        {
            MovieTitle = "Inception",
            StartTime = DateTime.UtcNow.AddHours(2),
            RoomNumber = "A1",
            TotalSeats = 150,
            TicketPrice = 45.50
        };
    }
}
