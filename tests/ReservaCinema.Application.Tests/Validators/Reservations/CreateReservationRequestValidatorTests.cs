using FluentValidation.TestHelper;
using ReservaCinema.Application.DTOs.Reservations;
using ReservaCinema.Application.Validators.Reservations;

namespace ReservaCinema.Application.Tests.Validators.Reservations;

public class CreateReservationRequestValidatorTests
{
    private readonly CreateReservationRequestValidator _validator = new();

    [Fact]
    public void Validate_ComDadosValidos_NaoDeveHaverErros()
    {
        var request = new CreateReservationRequest
        {
            SessionId = Guid.NewGuid(),
            UserId = "user-123",
            SeatNumbers = ["A1", "A2"]
        };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ComSessionIdVazio_DeveRetornarErro()
    {
        var request = new CreateReservationRequest
        {
            SessionId = Guid.Empty,
            UserId = "user-123",
            SeatNumbers = ["A1"]
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.SessionId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ComUserIdInvalido_DeveRetornarErro(string userId)
    {
        var request = new CreateReservationRequest
        {
            SessionId = Guid.NewGuid(),
            UserId = userId,
            SeatNumbers = ["A1"]
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validate_SemAssentos_DeveRetornarErro()
    {
        var request = new CreateReservationRequest
        {
            SessionId = Guid.NewGuid(),
            UserId = "user-123",
            SeatNumbers = []
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.SeatNumbers);
    }

    [Fact]
    public void Validate_ComMaisDe10Assentos_DeveRetornarErro()
    {
        var request = new CreateReservationRequest
        {
            SessionId = Guid.NewGuid(),
            UserId = "user-123",
            SeatNumbers = Enumerable.Range(1, 11).Select(i => $"A{i}").ToArray()
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.SeatNumbers);
    }

    [Fact]
    public void Validate_ComAssentosDuplicados_DeveRetornarErro()
    {
        var request = new CreateReservationRequest
        {
            SessionId = Guid.NewGuid(),
            UserId = "user-123",
            SeatNumbers = ["A1", "A2", "A1"]
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.SeatNumbers);
    }

    [Fact]
    public void Validate_Com10Assentos_NaoDeveHaverErros()
    {
        var request = new CreateReservationRequest
        {
            SessionId = Guid.NewGuid(),
            UserId = "user-123",
            SeatNumbers = Enumerable.Range(1, 10).Select(i => $"A{i}").ToArray()
        };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
