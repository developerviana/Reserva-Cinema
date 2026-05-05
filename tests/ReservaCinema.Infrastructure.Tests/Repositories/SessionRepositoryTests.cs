using Microsoft.EntityFrameworkCore;
using ReservaCinema.Infrastructure.Persistence;
using ReservaCinema.Infrastructure.Persistence.Repositories;
using ReservaCinema.Tests.Shared.Builders;

namespace ReservaCinema.Infrastructure.Tests.Repositories;

public class SessionRepositoryTests
{
    private static ReservaCinemaDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ReservaCinemaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ReservaCinemaDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ComSessaoValida_DevePersistirERetornar()
    {
        // Arrange
        await using var context = CreateDbContext();
        var repository = new SessionRepository(context);
        var session = new SessionBuilder().Build();

        // Act
        var result = await repository.AddAsync(session);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(session.Id);
        context.Sessions.Count().Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_ComIdExistente_DeveRetornarSessao()
    {
        // Arrange
        await using var context = CreateDbContext();
        var repository = new SessionRepository(context);
        var session = new SessionBuilder().WithMovieTitle("Matrix").Build();
        await repository.AddAsync(session);

        // Act
        var result = await repository.GetByIdAsync(session.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(session.Id);
        result.MovieTitle.Should().Be("Matrix");
    }

    [Fact]
    public async Task GetByIdAsync_ComIdInexistente_DeveRetornarNull()
    {
        // Arrange
        await using var context = CreateDbContext();
        var repository = new SessionRepository(context);

        // Act
        var result = await repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_DeveRetornarApenasSesoesAtivas()
    {
        // Arrange
        await using var context = CreateDbContext();
        var repository = new SessionRepository(context);

        var ativa = new SessionBuilder().WithMovieTitle("Ativa").Build();
        var inativa = new SessionBuilder().WithMovieTitle("Inativa").WithIsActive(false).Build();
        await repository.AddAsync(ativa);
        await repository.AddAsync(inativa);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(1);
        result.Single().MovieTitle.Should().Be("Ativa");
    }

    [Fact]
    public async Task GetAllAsync_DeveOrdenarPorStartTime()
    {
        // Arrange
        await using var context = CreateDbContext();
        var repository = new SessionRepository(context);

        var segunda = new SessionBuilder().WithStartTime(DateTime.UtcNow.AddHours(2)).Build();
        var primeira = new SessionBuilder().WithStartTime(DateTime.UtcNow.AddHours(1)).Build();
        await repository.AddAsync(segunda);
        await repository.AddAsync(primeira);

        // Act
        var result = (await repository.GetAllAsync()).ToList();

        // Assert
        result[0].StartTime.Should().BeBefore(result[1].StartTime);
    }

    [Fact]
    public async Task UpdateAsync_ComSessaoExistente_DeveAtualizarCampos()
    {
        // Arrange
        await using var context = CreateDbContext();
        var repository = new SessionRepository(context);
        var session = new SessionBuilder().WithMovieTitle("Original").Build();
        await repository.AddAsync(session);

        session.MovieTitle = "Atualizado";
        session.UpdatedAt = DateTime.UtcNow;

        // Act
        var result = await repository.UpdateAsync(session);

        // Assert
        result.MovieTitle.Should().Be("Atualizado");
        var persisted = await context.Sessions.FindAsync(session.Id);
        persisted!.MovieTitle.Should().Be("Atualizado");
    }

    [Fact]
    public async Task DeleteAsync_ComIdExistente_DeveFazerSoftDelete()
    {
        // Arrange
        await using var context = CreateDbContext();
        var repository = new SessionRepository(context);
        var session = new SessionBuilder().Build();
        await repository.AddAsync(session);

        // Act
        var result = await repository.DeleteAsync(session.Id);

        // Assert
        result.Should().BeTrue();
        var persisted = await context.Sessions.FindAsync(session.Id);
        persisted!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_ComIdInexistente_DeveRetornarFalse()
    {
        // Arrange
        await using var context = CreateDbContext();
        var repository = new SessionRepository(context);

        // Act
        var result = await repository.DeleteAsync(Guid.NewGuid());

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsAsync_ComIdExistente_DeveRetornarTrue()
    {
        // Arrange
        await using var context = CreateDbContext();
        var repository = new SessionRepository(context);
        var session = new SessionBuilder().Build();
        await repository.AddAsync(session);

        // Act
        var result = await repository.ExistsAsync(session.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ComIdInexistente_DeveRetornarFalse()
    {
        // Arrange
        await using var context = CreateDbContext();
        var repository = new SessionRepository(context);

        // Act
        var result = await repository.ExistsAsync(Guid.NewGuid());

        // Assert
        result.Should().BeFalse();
    }
}
