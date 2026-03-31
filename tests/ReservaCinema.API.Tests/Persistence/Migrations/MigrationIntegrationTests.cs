using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ReservaCinema.Domain.Entities;
using ReservaCinema.Infrastructure.Persistence;

namespace ReservaCinema.API.Tests.Persistence.Migrations;

public class MigrationIntegrationTests
{
    [Fact]
    public async Task DbContext_ShouldAllowCrudOperations()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ReservaCinemaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var context = new ReservaCinemaDbContext(options);

        // Act: INSERT
        var session = new Session(
            movieTitle: "Batman",
            roomNumber: "Room 1",
            startTime: DateTime.UtcNow,
            totalSeats: 100,
            ticketPrice: 25.50m
        );

        context.Sessions.Add(session);
        await context.SaveChangesAsync();

        // Assert: SELECT
        var saved = await context.Sessions.FirstOrDefaultAsync();
        saved.Should().NotBeNull();
        saved!.MovieTitle.Should().Be("Batman");
        saved.RoomNumber.Should().Be("Room 1");
        saved.TotalSeats.Should().Be(100);
    }

    [Fact]
    public async Task DbContext_ShouldAllowMultipleSessions()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ReservaCinemaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var context = new ReservaCinemaDbContext(options);

        // Act
        context.Sessions.Add(new Session("Movie1", "R1", DateTime.UtcNow, 50, 20m));
        context.Sessions.Add(new Session("Movie2", "R2", DateTime.UtcNow, 60, 25m));
        await context.SaveChangesAsync();

        // Assert
        var count = await context.Sessions.CountAsync();
        count.Should().Be(2);
    }
}