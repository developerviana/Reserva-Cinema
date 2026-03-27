using FluentValidation;
using ReservaCinema.Application.DTOs.Sessions;

namespace ReservaCinema.Application.Validators.Sessions;

/// <summary>
/// Validador para CreateSessionRequest usando FluentValidation.
/// </summary>
public class CreateSessionRequestValidator : AbstractValidator<CreateSessionRequest>
{
    public CreateSessionRequestValidator()
    {
        RuleFor(x => x.MovieTitle)
            .NotEmpty()
            .WithMessage("O título do filme é obrigatório")
            .MinimumLength(3)
            .WithMessage("O título do filme deve ter no mínimo 3 caracteres")
            .MaximumLength(255)
            .WithMessage("O título do filme não pode exceder 255 caracteres");

        RuleFor(x => x.StartTime)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("O horário da sessão não pode ser no passado");

        RuleFor(x => x.RoomNumber)
            .NotEmpty()
            .WithMessage("O número da sala é obrigatório")
            .MinimumLength(2)
            .WithMessage("O número da sala deve ter no mínimo 2 caracteres")
            .MaximumLength(10)
            .WithMessage("O número da sala não pode exceder 10 caracteres");

        RuleFor(x => x.TotalSeats)
            .GreaterThan(0)
            .WithMessage("O total de assentos deve ser maior que 0")
            .LessThanOrEqualTo(10000)
            .WithMessage("O total de assentos não pode exceder 10.000");

        RuleFor(x => x.TicketPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("O preço do ingresso não pode ser negativo")
            .LessThanOrEqualTo(999.99m)
            .WithMessage("O preço do ingresso não pode exceder R$ 999,99");
    }
}
