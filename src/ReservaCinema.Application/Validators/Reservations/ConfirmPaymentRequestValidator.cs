using FluentValidation;
using ReservaCinema.Application.DTOs.Reservations;

namespace ReservaCinema.Application.Validators.Reservations;

public class ConfirmPaymentRequestValidator : AbstractValidator<ConfirmPaymentRequest>
{
    public ConfirmPaymentRequestValidator()
    {
        RuleFor(x => x.PaymentMethod)
            .NotEmpty().WithMessage("Método de pagamento é obrigatório.");

        RuleFor(x => x.TransactionId)
            .NotEmpty().WithMessage("ID da transação é obrigatório.");
    }
}
