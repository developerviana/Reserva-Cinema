using Moq;
using ReservaCinema.Domain.Repositories;
using ReservaCinema.Tests.Shared.Builders;
using ReservaCinema.Tests.Shared.Mocks;

namespace ReservaCinema.Infrastructure.Tests.Repositories;

/// <summary>
/// Testes para o repositório de sessões.
/// </summary>
public class SessionRepositoryTests
{
    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnSession()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var mockRepository = RepositoryMockFactory.CreateSessionRepositoryMock();
        
        var expectedSession = new SessionBuilder()
            .WithId(sessionId)
            .WithMovieTitle("Test Movie")
            .Build();
        
        mockRepository.Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(expectedSession);

        // Act
        var result = await mockRepository.Object.GetByIdAsync(sessionId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(sessionId);
        result.MovieTitle.Should().Be("Test Movie");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var invalidId = Guid.NewGuid();
        var mockRepository = RepositoryMockFactory.CreateSessionRepositoryMock();

        // Act
        var result = await mockRepository.Object.GetByIdAsync(invalidId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WithValidSession_ShouldReturnSession()
    {
        // Arrange
        var session = new SessionBuilder().Build();
        var mockRepository = RepositoryMockFactory.CreateSessionRepositoryMock();
        
        mockRepository.Setup(r => r.AddAsync(It.IsAny<Session>()))
            .ReturnsAsync((Session s) => s);

        // Act
        var result = await mockRepository.Object.AddAsync(session);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(session.Id);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllSessions()
    {
        // Arrange
        var sessions = new List<Session>
        {
            new SessionBuilder().WithMovieTitle("Movie 1").Build(),
            new SessionBuilder().WithMovieTitle("Movie 2").Build(),
            new SessionBuilder().WithMovieTitle("Movie 3").Build()
        };
        
        var mockRepository = RepositoryMockFactory.CreateSessionRepositoryMock();
        mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(sessions);

        // Act
        var result = await mockRepository.Object.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(s => s.MovieTitle == "Movie 1");
        result.Should().Contain(s => s.MovieTitle == "Movie 2");
        result.Should().Contain(s => s.MovieTitle == "Movie 3");
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var mockRepository = RepositoryMockFactory.CreateSessionRepositoryMock();
        
        mockRepository.Setup(r => r.DeleteAsync(sessionId))
            .ReturnsAsync(true);

        // Act
        var result = await mockRepository.Object.DeleteAsync(sessionId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var mockRepository = RepositoryMockFactory.CreateSessionRepositoryMock();
        
        mockRepository.Setup(r => r.ExistsAsync(sessionId))
            .ReturnsAsync(true);

        // Act
        var result = await mockRepository.Object.ExistsAsync(sessionId);

        // Assert
        result.Should().BeTrue();
    }
}
