namespace ReservaCinema.Tests.Shared.Fixtures;

/// <summary>
/// Fixture base para testes com setup/teardown comum.
/// </summary>
public abstract class BaseFixture : IAsyncLifetime
{
    public virtual async Task InitializeAsync()
    {
        await Task.CompletedTask;
    }

    public virtual async Task DisposeAsync()
    {
        await Task.CompletedTask;
    }
}
