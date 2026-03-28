using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ReservaCinema.Domain.Entities;
using ReservaCinema.Infrastructure.Persistence;

namespace ReservaCinema.API.Tests.Persistence.Migrations;

public class MigrationIntegrationTests
{
    private readonly string _connectionString =
        "Host=localhost;Port=5432;Database=cinema_db_test;Username=postgres;Password=postgres";

    [Fact]
    public async Task Database_ShouldApplyMigrations_AndAllowCrudOperations()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ReservaCinemaDbContext>()
            .UseNpgsql(_connectionString)
            .Options;

        await using var context = new ReservaCinemaDbContext(options);

        // Garantir banco limpo
        await context.Database.EnsureDeletedAsync();

        // Act
        await context.Database.MigrateAsync();

        // Assert 1: Migration aplicada
        var canConnect = await context.Database.CanConnectAsync();
        canConnect.Should().BeTrue();

        // Assert 2: INSERT
        var session = new Session(
            movieTitle: "Batman",
            roomNumber: "Room 1",
            startTime: DateTime.UtcNow,
            totalSeats: 100,
            ticketPrice: 25.50m
        );

        context.Sessions.Add(session);
        await context.SaveChangesAsync();

        // Assert 3: SELECT
        var saved = await context.Sessions.FirstOrDefaultAsync();

        saved.Should().NotBeNull();
        saved!.MovieTitle.Should().Be("Batman");

        // Assert 4: UPDATE
        saved.MovieTitle = "Batman - Updated";
        await context.SaveChangesAsync();

        var updated = await context.Sessions.FirstAsync();
        updated.MovieTitle.Should().Be("Batman - Updated");

        // Assert 5: DELETE
        context.Sessions.Remove(updated);
        await context.SaveChangesAsync();

        var count = await context.Sessions.CountAsync();
        count.Should().Be(0);
    }
}