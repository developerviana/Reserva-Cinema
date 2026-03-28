using ReservaCinema.Application.DTOs.Sessions;
using ReservaCinema.Tests.Shared.Builders;
using ReservaCinema.Tests.Shared.Constants;

namespace ReservaCinema.Application.Tests.DTOs;

/// <summary>
/// Testes para o DTO CreateSessionRequest.
/// </summary>
public class CreateSessionRequestTests
{
    [Fact]
    public void Should_Set_Properties_Correctly()
    {
        var request = new CreateSessionRequestBuilder()
            .WithMovieTitle("Batman")
            .WithRoomNumber("A1")
            .Build();

        request.MovieTitle.Should().Be("Batman");
        request.RoomNumber.Should().Be("A1");
    }

}
