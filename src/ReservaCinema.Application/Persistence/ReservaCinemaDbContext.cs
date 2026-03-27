using Microsoft.EntityFrameworkCore;
using ReservaCinema.Application.Entities;

namespace ReservaCinema.Application.Persistence;

public class ReservaCinemaDbContext : DbContext
{
    public ReservaCinemaDbContext(DbContextOptions<ReservaCinemaDbContext> options) 
        : base(options)
    {
    }

    public DbSet<Session> Sessions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.MovieTitle)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.RoomNumber)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(e => e.StartTime)
                .IsRequired();

            entity.Property(e => e.TotalSeats)
                .IsRequired();

            entity.Property(e => e.TicketPrice)
                .IsRequired()
                .HasPrecision(10, 2);

            entity.Property(e => e.AvailableSeats)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}
