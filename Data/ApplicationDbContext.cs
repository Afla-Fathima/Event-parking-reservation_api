using EventParkingReservation.Models;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservation.Data
{
    public class ApplicationDbContext
        : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext>
                options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }

        public DbSet<Seat> Seats { get; set; }

        public DbSet<BookingSeat>
            BookingSeats
        { get; set; }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Seat>()
                .HasIndex(s => new
                {
                    s.EventId,
                    s.SeatNumber
                })
                .IsUnique();

            modelBuilder.Entity<Seat>()
                .HasOne(s => s.Event)
                .WithMany()
                .HasForeignKey(s =>
                    s.EventId)
                .OnDelete(
                    DeleteBehavior.Cascade
                );
        }
    }
}