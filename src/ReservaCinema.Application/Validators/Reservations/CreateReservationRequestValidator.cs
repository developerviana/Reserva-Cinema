using FluentValidation;
using ReservaCinema.Application.DTOs.Reservations;

namespace ReservaCinema.Application.Validators.Reservations;

public class CreateReservationRequestValidator : AbstractValidator<CreateReservationRequest>
{
    public CreateReservationRequestValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEmpty()
            .WithMessage("SessionId é obrigatório");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId é obrigatório");

        RuleFor(x => x.SeatNumbers)
            .NotEmpty()
            .WithMessage("Deve haver pelo menos um assento")
            .Must(s => s.Length <= 10)
            .WithMessage("Máximo de 10 assentos por reserva")
            .Must(s => s.Distinct().Count() == s.Length)
            .WithMessage("Não pode haver assentos duplicados");
    }
}
