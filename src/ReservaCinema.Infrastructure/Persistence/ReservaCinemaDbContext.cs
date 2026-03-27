using Microsoft.EntityFrameworkCore;
using ReservaCinema.Domain.Entities;

namespace ReservaCinema.Infrastructure.Persistence;

/// <summary>
/// DbContext principal da aplicação Reserva Cinema.
/// </summary>
public class ReservaCinemaDbContext : DbContext
{
    public ReservaCinemaDbContext(DbContextOptions<ReservaCinemaDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Tabela de sessões de cinema.
    /// </summary>
    public DbSet<Session> Sessions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurar Session
        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.MovieTitle)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.RoomNumber)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(e => e.TicketPrice)
                .HasPrecision(10, 2);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValue(DateTime.UtcNow);

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);

            entity.Property(e => e.RatingClassification)
                .HasMaxLength(2);

            // Índices para melhorar performance
            entity.HasIndex(e => e.StartTime);
            entity.HasIndex(e => e.RoomNumber);
            entity.HasIndex(e => e.IsActive);
        });
    }
}
