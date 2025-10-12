using Core.Entities;
using Infrastructure.Config;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class TicketContext(DbContextOptions<TicketContext> options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Show> Shows { get; set; }
    public DbSet<Seat> Seats { get; set; }
    public DbSet<ReservationSeat> ReservationSeats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReservationConfiguration).Assembly);

        modelBuilder.Entity<ReservationSeat>()
            .HasIndex(rs => new { rs.ShowId, rs.SeatId })
            .IsUnique();

        modelBuilder.Entity<ReservationSeat>()
            .HasIndex(rs => rs.ReservationId);  

        modelBuilder.Entity<ReservationSeat>()
            .HasIndex(rs => rs.ShowId);
    }
}

