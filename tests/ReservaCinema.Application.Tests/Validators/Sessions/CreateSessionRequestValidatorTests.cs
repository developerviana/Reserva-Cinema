using FluentValidation.TestHelper;
using ReservaCinema.Application.DTOs.Sessions;
using ReservaCinema.Application.Validators.Sessions;
using ReservaCinema.Tests.Shared.Builders;

namespace ReservaCinema.Application.Tests.Validators.Sessions;

/// <summary>
/// Testes para o validador CreateSessionRequestValidator.
/// Valida as regras de negócio aplicadas à criação de sessões de cinema.
/// </summary>
public class CreateSessionRequestValidatorTests
{
    private readonly CreateSessionRequestValidator _validator;

    public CreateSessionRequestValidatorTests()
    {
        _validator = new CreateSessionRequestValidator();
    }

    #region MovieTitle Validation

    [Fact]
    public void Deve_Falhar_Quando_Titulo_For_Vazio()
    {
        var request = new CreateSessionRequestBuilder()
            .WithMovieTitle(string.Empty)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.MovieTitle);
    }

    [Fact]
    public void Deve_Falhar_Quando_Titulo_For_Menor_Que_Minimo()
    {
        var request = new CreateSessionRequestBuilder()
            .WithMovieTitle("AB")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.MovieTitle);
    }

    [Fact]
    public void Deve_Falhar_Quando_Titulo_Exceder_Maximo()
    {
        var request = new CreateSessionRequestBuilder()
            .WithMovieTitle(new string('A', 256))
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.MovieTitle);
    }

    [Theory]
    [InlineData("ABC")]
    [InlineData("The Matrix")]
    [InlineData("Avatar 2")]
    public void Deve_Passar_Quando_Titulo_For_Valido(string movieTitle)
    {
        var request = new CreateSessionRequestBuilder()
            .WithMovieTitle(movieTitle)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.MovieTitle);
    }

    #endregion

    #region StartTime Validation

    [Fact]
    public void Deve_Falhar_Quando_Data_For_No_Passado()
    {
        var now = DateTime.UtcNow;

        var request = new CreateSessionRequestBuilder()
            .WithStartTime(now.AddHours(-1))
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.StartTime);
    }

    [Fact]
    public void Deve_Passar_Quando_Data_For_No_Futuro()
    {
        var now = DateTime.UtcNow;

        var request = new CreateSessionRequestBuilder()
            .WithStartTime(now.AddHours(2))
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.StartTime);
    }

    #endregion

    #region RoomNumber Validation

    [Fact]
    public void Deve_Falhar_Quando_Sala_For_Vazia()
    {
        var request = new CreateSessionRequestBuilder()
            .WithRoomNumber(string.Empty)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.RoomNumber);
    }

    [Fact]
    public void Deve_Falhar_Quando_Sala_For_Menor_Que_Minimo()
    {
        var request = new CreateSessionRequestBuilder()
            .WithRoomNumber("A")
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.RoomNumber);
    }

    [Fact]
    public void Deve_Falhar_Quando_Sala_Exceder_Maximo()
    {
        var request = new CreateSessionRequestBuilder()
            .WithRoomNumber(new string('A', 11))
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.RoomNumber);
    }

    [Theory]
    [InlineData("A1")]
    [InlineData("AB")]
    [InlineData("ROOM01")]
    public void Deve_Passar_Quando_Sala_For_Valida(string roomNumber)
    {
        var request = new CreateSessionRequestBuilder()
            .WithRoomNumber(roomNumber)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.RoomNumber);
    }

    #endregion

    #region TotalSeats Validation

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Deve_Falhar_Quando_TotalAssentos_For_Invalido(int totalSeats)
    {
        var request = new CreateSessionRequestBuilder()
            .WithTotalSeats(totalSeats)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.TotalSeats);
    }

    [Fact]
    public void Deve_Falhar_Quando_TotalAssentos_Exceder_Maximo()
    {
        var request = new CreateSessionRequestBuilder()
            .WithTotalSeats(10001)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.TotalSeats);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(10000)]
    public void Deve_Passar_Quando_TotalAssentos_For_Valido(int totalSeats)
    {
        var request = new CreateSessionRequestBuilder()
            .WithTotalSeats(totalSeats)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.TotalSeats);
    }

    #endregion

    #region TicketPrice Validation

    [Fact]
    public void Deve_Falhar_Quando_Preco_For_Negativo()
    {
        var request = new CreateSessionRequestBuilder()
            .WithTicketPrice(-10m)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.TicketPrice);
    }

    [Fact]
    public void Deve_Falhar_Quando_Preco_Exceder_Maximo()
    {
        var request = new CreateSessionRequestBuilder()
            .WithTicketPrice(1000m)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.TicketPrice);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10.5)]
    [InlineData(50)]
    [InlineData(999.99)]
    public void Deve_Passar_Quando_Preco_For_Valido(decimal price)
    {
        var request = new CreateSessionRequestBuilder()
            .WithTicketPrice(price)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.TicketPrice);
    }

    #endregion

    #region Multiple Fields

    [Fact]
    public void Deve_Falhar_Quando_Multiplos_Campos_Invalidos()
    {
        var now = DateTime.UtcNow;

        var request = new CreateSessionRequestBuilder()
            .WithMovieTitle("AB")
            .WithStartTime(now.AddHours(-1))
            .WithRoomNumber("A")
            .WithTotalSeats(0)
            .WithTicketPrice(-5m)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.MovieTitle);
        result.ShouldHaveValidationErrorFor(x => x.StartTime);
        result.ShouldHaveValidationErrorFor(x => x.RoomNumber);
        result.ShouldHaveValidationErrorFor(x => x.TotalSeats);
        result.ShouldHaveValidationErrorFor(x => x.TicketPrice);
    }

    [Fact]
    public void Deve_Passar_Quando_Todos_Campos_Forem_Validos()
    {
        var now = DateTime.UtcNow;

        var request = new CreateSessionRequestBuilder()
            .WithMovieTitle("Inception")
            .WithStartTime(now.AddDays(1))
            .WithRoomNumber("A1")
            .WithTotalSeats(150)
            .WithTicketPrice(35.5m)
            .Build();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion
}