using FluentValidation.TestHelper;
using ReservaCinema.Application.DTOs.Reservations;
using ReservaCinema.Application.Validators.Reservations;

namespace ReservaCinema.Application.Tests.Validators.Reservations;

public class ConfirmPaymentRequestValidatorTests
{
    private readonly ConfirmPaymentRequestValidator _validator = new();

    [Fact]
    public void Validate_ComDadosValidos_NaoDeveHaverErros()
    {
        var request = new ConfirmPaymentRequest
        {
            PaymentMethod = "credit_card",
            TransactionId = "tx-456"
        };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ComPaymentMethodInvalido_DeveRetornarErro(string paymentMethod)
    {
        var request = new ConfirmPaymentRequest
        {
            PaymentMethod = paymentMethod,
            TransactionId = "tx-456"
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.PaymentMethod);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ComTransactionIdInvalido_DeveRetornarErro(string transactionId)
    {
        var request = new ConfirmPaymentRequest
        {
            PaymentMethod = "credit_card",
            TransactionId = transactionId
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.TransactionId);
    }
}
