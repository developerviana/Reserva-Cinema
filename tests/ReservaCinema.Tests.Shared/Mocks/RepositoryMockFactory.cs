using Moq;
using ReservaCinema.Application.Persistence.Repositories;

namespace ReservaCinema.Tests.Shared.Mocks;

/// <summary>
/// Factory para criar mocks de repositórios.
/// </summary>
public static class RepositoryMockFactory
{
    /// <summary>
    /// Cria um mock do ISessionRepository.
    /// </summary>
    public static Mock<ISessionRepository> CreateSessionRepositoryMock()
    {
        return new Mock<ISessionRepository>();
    }
}
