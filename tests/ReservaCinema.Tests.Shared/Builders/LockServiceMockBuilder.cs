using Moq;
using ReservaCinema.Application.Services;

namespace ReservaCinema.Tests.Shared.Builders;

/// <summary>
/// Builder para Mock&lt;IDistributedLockService&gt;.
/// Use este builder em testes de Application e API — não mocka Redis diretamente.
/// Para testes de Infrastructure (RedisLockService), use RedisLockServiceMockBuilder.
/// </summary>
public class LockServiceMockBuilder
{
    private string? _acquireResult = Guid.NewGuid().ToString();
    private bool _releaseResult = true;
    private bool _isOwnedResult = true;

    public LockServiceMockBuilder ComAquisicaoBemSucedida()
    {
        _acquireResult = Guid.NewGuid().ToString();
        return this;
    }

    public LockServiceMockBuilder ComAquisicaoFalhada()
    {
        _acquireResult = null;
        return this;
    }

    public LockServiceMockBuilder ComLiberacaoFalhada()
    {
        _releaseResult = false;
        return this;
    }

    public LockServiceMockBuilder ComLockNaoPertencenteAoToken()
    {
        _isOwnedResult = false;
        return this;
    }

    public Mock<IDistributedLockService> Build()
    {
        var mock = new Mock<IDistributedLockService>();

        mock.Setup(x => x.AcquireLockAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .ReturnsAsync(_acquireResult);

        mock.Setup(x => x.ReleaseLockAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_releaseResult);

        mock.Setup(x => x.IsLockOwnedAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_isOwnedResult);

        return mock;
    }

    public static LockServiceMockBuilder Sucesso() => new LockServiceMockBuilder().ComAquisicaoBemSucedida();
    public static LockServiceMockBuilder Conflito() => new LockServiceMockBuilder().ComAquisicaoFalhada();
}
