using Moq;
using ReservaCinema.Domain.Repositories;

namespace ReservaCinema.Tests.Shared.Mocks;

/// <summary>
/// Factory para criar mocks de repositórios comumente usados em testes.
/// </summary>
public class RepositoryMockFactory
{
    public static Mock<ISessionRepository> CreateSessionRepositoryMock()
    {
        var mock = new Mock<ISessionRepository>();
        
        // Configure comportamentos padrão
        mock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Session?)null);
        
        mock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(Enumerable.Empty<Session>());
        
        mock.Setup(r => r.ExistsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(false);
        
        mock.Setup(r => r.DeleteAsync(It.IsAny<Guid>()))
            .ReturnsAsync(false);

        return mock;
    }
}
