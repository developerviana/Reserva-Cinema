using ReservaCinema.Application.DTOs.Sessions;
using ReservaCinema.Application.Validators.Sessions;

namespace ReservaCinema.Application.Validators.Sessions;

public class CreateSessionRequestValidator
{
    public ValidationResult Validate(CreateSessionRequest? request)
    {
        var result = new ValidationResult { IsValid = true };

        if (request == null)
        {
            result.IsValid = false;
            result.Errors.Add(new ValidationError { PropertyName = "Request", Message = "Request cannot be null" });
            return result;
        }

        // MovieTitle validation
        if (string.IsNullOrWhiteSpace(request.MovieTitle))
        {
            result.IsValid = false;
            result.Errors.Add(new ValidationError { PropertyName = "MovieTitle", Message = "MovieTitle is required" });
        }

        // StartTime validation
        if (request.StartTime < DateTime.UtcNow)
        {
            result.IsValid = false;
            result.Errors.Add(new ValidationError { PropertyName = "StartTime", Message = "StartTime must be in the future" });
        }

        // RoomNumber validation
        if (string.IsNullOrWhiteSpace(request.RoomNumber) || request.RoomNumber.Length < 2)
        {
            result.IsValid = false;
            result.Errors.Add(new ValidationError { PropertyName = "RoomNumber", Message = "RoomNumber must have at least 2 characters" });
        }

        // TotalSeats validation
        if (request.TotalSeats <= 0)
        {
            result.IsValid = false;
            result.Errors.Add(new ValidationError { PropertyName = "TotalSeats", Message = "TotalSeats must be greater than 0" });
        }

        // TicketPrice validation
        if (request.TicketPrice < 0)
        {
            result.IsValid = false;
            result.Errors.Add(new ValidationError { PropertyName = "TicketPrice", Message = "TicketPrice cannot be negative" });
        }

        return result;
    }
}
