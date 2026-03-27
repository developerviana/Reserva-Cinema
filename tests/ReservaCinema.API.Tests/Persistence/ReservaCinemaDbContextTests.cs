using Microsoft.EntityFrameworkCore;
using ReservaCinema.Application.Persistence;
using ReservaCinema.Application.Entities;

namespace ReservaCinema.API.Tests.Persistence;

public class ReservaCinemaDbContextTests
{
    [Fact]
    public async Task DbContext_ShouldCreateDatabase_WhenOptionsProvided()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ReservaCinemaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        // Act & Assert
        using (var context = new ReservaCinemaDbContext(options))
        {
            Assert.NotNull(context);
            Assert.NotNull(context.Sessions);
        }
    }

    [Fact]
    public async Task SaveSession_ShouldPersistData_WhenValidSessionProvided()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ReservaCinemaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var session = new Session
        {
            MovieTitle = "The Matrix",
            StartTime = DateTime.UtcNow.AddHours(2),
            RoomNumber = "A1",
            TotalSeats = 100,
            TicketPrice = 25.50,
            AvailableSeats = 100,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        using (var context = new ReservaCinemaDbContext(options))
        {
            context.Sessions.Add(session);
            await context.SaveChangesAsync();
        }

        // Assert
        using (var context = new ReservaCinemaDbContext(options))
        {
            var savedSession = await context.Sessions.FirstOrDefaultAsync(s => s.MovieTitle == "The Matrix");
            Assert.NotNull(savedSession);
            Assert.Equal("The Matrix", savedSession.MovieTitle);
            Assert.Equal("A1", savedSession.RoomNumber);
            Assert.Equal(100, savedSession.TotalSeats);
        }
    }

    [Fact]
    public async Task SaveMultipleSessions_ShouldPersistAll_WhenValidDataProvided()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ReservaCinemaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var sessions = new List<Session>
        {
            new()
            {
                MovieTitle = "Inception",
                StartTime = DateTime.UtcNow.AddHours(1),
                RoomNumber = "A1",
                TotalSeats = 80,
                TicketPrice = 30.00,
                AvailableSeats = 80,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                MovieTitle = "Interstellar",
                StartTime = DateTime.UtcNow.AddHours(3),
                RoomNumber = "B2",
                TotalSeats = 120,
                TicketPrice = 35.00,
                AvailableSeats = 120,
                CreatedAt = DateTime.UtcNow
            }
        };

        // Act
        using (var context = new ReservaCinemaDbContext(options))
        {
            context.Sessions.AddRange(sessions);
            await context.SaveChangesAsync();
        }

        // Assert
        using (var context = new ReservaCinemaDbContext(options))
        {
            var allSessions = await context.Sessions.ToListAsync();
            Assert.Equal(2, allSessions.Count);
        }
    }

    [Fact]
    public async Task UpdateSession_ShouldModifyData_WhenChangesApplied()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ReservaCinemaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var session = new Session
        {
            MovieTitle = "Avatar",
            StartTime = DateTime.UtcNow.AddHours(2),
            RoomNumber = "C3",
            TotalSeats = 150,
            TicketPrice = 40.00,
            AvailableSeats = 150,
            CreatedAt = DateTime.UtcNow
        };

        using (var context = new ReservaCinemaDbContext(options))
        {
            context.Sessions.Add(session);
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = new ReservaCinemaDbContext(options))
        {
            var existingSession = await context.Sessions.FirstAsync();
            existingSession.AvailableSeats = 120;
            await context.SaveChangesAsync();
        }

        // Assert
        using (var context = new ReservaCinemaDbContext(options))
        {
            var updatedSession = await context.Sessions.FirstAsync();
            Assert.Equal(120, updatedSession.AvailableSeats);
        }
    }

    [Fact]
    public async Task DeleteSession_ShouldRemoveData_WhenSessionRemoved()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ReservaCinemaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var session = new Session
        {
            MovieTitle = "Titanic",
            StartTime = DateTime.UtcNow.AddHours(2),
            RoomNumber = "D4",
            TotalSeats = 200,
            TicketPrice = 28.00,
            AvailableSeats = 200,
            CreatedAt = DateTime.UtcNow
        };

        using (var context = new ReservaCinemaDbContext(options))
        {
            context.Sessions.Add(session);
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = new ReservaCinemaDbContext(options))
        {
            var existingSession = await context.Sessions.FirstAsync();
            context.Sessions.Remove(existingSession);
            await context.SaveChangesAsync();
        }

        // Assert
        using (var context = new ReservaCinemaDbContext(options))
        {
            var deletedSession = await context.Sessions.FirstOrDefaultAsync();
            Assert.Null(deletedSession);
        }
    }
}
