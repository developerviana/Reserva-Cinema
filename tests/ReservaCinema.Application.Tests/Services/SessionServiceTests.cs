using Moq;
using ReservaCinema.Application.DTOs.Sessions;
using ReservaCinema.Application.Persistence.Repositories;
using ReservaCinema.Application.Services;
using ReservaCinema.Domain.Entities;
using ReservaCinema.Tests.Shared.Builders;

namespace ReservaCinema.Application.Tests.Services;

public class SessionServiceTests
{
    private readonly Mock<ISessionRepository> _mockRepository;
    private readonly SessionService _service;

    public SessionServiceTests()
    {
        _mockRepository = new Mock<ISessionRepository>();
        _service = new SessionService(_mockRepository.Object);
    }

    #region CreateSessionAsync

    [Fact]
    public async Task CreateSessionAsync_ComDadosValidos_DeveRetornarSessaoCriada()
    {
        // Arrange
        var request = new CreateSessionRequestBuilder().Build();
        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Session>()))
            .ReturnsAsync((Session s) => s);

        // Act
        var result = await _service.CreateSessionAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.MovieTitle.Should().Be(request.MovieTitle);
        result.RoomNumber.Should().Be(request.RoomNumber);
        result.TotalSeats.Should().Be(request.TotalSeats);
        result.AvailableSeats.Should().Be(request.TotalSeats);
        result.TicketPrice.Should().Be(request.TicketPrice);
    }

    [Fact]
    public async Task CreateSessionAsync_DeveChamarRepositorioUmaVez()
    {
        // Arrange
        var request = new CreateSessionRequestBuilder().Build();
        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Session>()))
            .ReturnsAsync((Session s) => s);

        // Act
        await _service.CreateSessionAsync(request);

        // Assert
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Session>()), Times.Once);
    }

    #endregion

    #region GetSessionByIdAsync

    [Fact]
    public async Task GetSessionByIdAsync_ComIdExistente_DeveRetornarSessao()
    {
        // Arrange
        var session = new SessionBuilder().WithMovieTitle("Matrix").Build();
        _mockRepository
            .Setup(r => r.GetByIdAsync(session.Id))
            .ReturnsAsync(session);

        // Act
        var result = await _service.GetSessionByIdAsync(session.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(session.Id);
        result.MovieTitle.Should().Be("Matrix");
    }

    [Fact]
    public async Task GetSessionByIdAsync_ComIdInexistente_DeveRetornarNull()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Session?)null);

        // Act
        var result = await _service.GetSessionByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetAllSessionsAsync

    [Fact]
    public async Task GetAllSessionsAsync_DeveRetornarTodasAsSessoes()
    {
        // Arrange
        var sessions = new List<Session>
        {
            new SessionBuilder().WithMovieTitle("Filme A").Build(),
            new SessionBuilder().WithMovieTitle("Filme B").Build(),
        };
        _mockRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(sessions);

        // Act
        var result = await _service.GetAllSessionsAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    #endregion

    #region UpdateSessionAsync

    [Fact]
    public async Task UpdateSessionAsync_ComIdExistente_DeveAtualizarERetornar()
    {
        // Arrange
        var session = new SessionBuilder().Build();
        var request = new CreateSessionRequestBuilder().WithMovieTitle("Novo Título").Build();
        _mockRepository.Setup(r => r.GetByIdAsync(session.Id)).ReturnsAsync(session);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Session>())).ReturnsAsync((Session s) => s);

        // Act
        var result = await _service.UpdateSessionAsync(session.Id, request);

        // Assert
        result.Should().NotBeNull();
        result!.MovieTitle.Should().Be("Novo Título");
    }

    [Fact]
    public async Task UpdateSessionAsync_ComIdInexistente_DeveRetornarNull()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Session?)null);

        // Act
        var result = await _service.UpdateSessionAsync(Guid.NewGuid(), new CreateSessionRequestBuilder().Build());

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region DeleteSessionAsync

    [Fact]
    public async Task DeleteSessionAsync_ComIdExistente_DeveRetornarTrue()
    {
        // Arrange
        var session = new SessionBuilder().Build();
        _mockRepository.Setup(r => r.GetByIdAsync(session.Id)).ReturnsAsync(session);
        _mockRepository.Setup(r => r.DeleteAsync(session.Id)).ReturnsAsync(true);

        // Act
        var result = await _service.DeleteSessionAsync(session.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteSessionAsync_ComIdInexistente_DeveRetornarFalse()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Session?)null);

        // Act
        var result = await _service.DeleteSessionAsync(Guid.NewGuid());

        // Assert
        result.Should().BeFalse();
    }

    #endregion
}
