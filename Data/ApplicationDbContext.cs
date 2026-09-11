using EventParkingReservation.Models;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservation.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // =====================================================
        // DB SETS
        // =====================================================

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Venue> Venues { get; set; }

        public DbSet<EventCategory> EventCategories { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<Seat> Seats { get; set; }

        public DbSet<ParkingSlot> ParkingSlots { get; set; }

        public DbSet<Booking> Bookings { get; set; }

        public DbSet<BookingSeat> BookingSeats { get; set; }

        public DbSet<ParkingReservation> ParkingReservations { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        // =====================================================
        // MODEL CONFIGURATION
        // =====================================================

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =================================================
            // UNIQUE INDEXES
            // =================================================

            modelBuilder.Entity<Customer>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<Booking>()
                .HasIndex(x => x.BookingNumber)
                .IsUnique();

            modelBuilder.Entity<Seat>()
                .HasIndex(x => new
                {
                    x.EventId,
                    x.SeatNumber
                })
                .IsUnique();

            modelBuilder.Entity<ParkingSlot>()
                .HasIndex(x => new
                {
                    x.EventId,
                    x.SlotNumber
                })
                .IsUnique();

            // =================================================
            // EVENT -> VENUE
            // =================================================

            modelBuilder.Entity<Event>()
                .HasOne(x => x.Venue)
                .WithMany(x => x.Events)
                .HasForeignKey(x => x.VenueId)
                .OnDelete(DeleteBehavior.Restrict);

            // =================================================
            // EVENT -> CATEGORY
            // =================================================

            modelBuilder.Entity<Event>()
                .HasOne(x => x.Category)
                .WithMany(x => x.Events)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // =================================================
            // EVENT -> SEATS
            // =================================================

            modelBuilder.Entity<Seat>()
                .HasOne(x => x.Event)
                .WithMany(x => x.Seats)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // =================================================
            // EVENT -> PARKING
            // =================================================

            modelBuilder.Entity<ParkingSlot>()
                .HasOne(x => x.Event)
                .WithMany(x => x.ParkingSlots)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // =================================================
            // CUSTOMER -> BOOKING
            // =================================================

            modelBuilder.Entity<Booking>()
                .HasOne(x => x.Customer)
                .WithMany(x => x.Bookings)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // =================================================
            // EVENT -> BOOKING
            // =================================================

            modelBuilder.Entity<Booking>()
                .HasOne(x => x.Event)
                .WithMany(x => x.Bookings)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            // =================================================
            // BOOKING -> BOOKING SEATS
            // =================================================

            modelBuilder.Entity<BookingSeat>()
                .HasOne(x => x.Booking)
                .WithMany(x => x.BookingSeats)
                .HasForeignKey(x => x.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // =================================================
            // SEAT -> BOOKING SEAT
            // =================================================

            modelBuilder.Entity<BookingSeat>()
                .HasOne(x => x.Seat)
                .WithMany(x => x.BookingSeats)
                .HasForeignKey(x => x.SeatId)
                .OnDelete(DeleteBehavior.Restrict);

            // =================================================
            // BOOKING -> PARKING RESERVATION
            // =================================================

            modelBuilder.Entity<ParkingReservation>()
                .HasOne(x => x.Booking)
                .WithOne(x => x.ParkingReservation)
                .HasForeignKey<ParkingReservation>(
                    x => x.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // =================================================
            // PARKING SLOT -> PARKING RESERVATION
            // =================================================

            modelBuilder.Entity<ParkingReservation>()
                .HasOne(x => x.ParkingSlot)
                .WithMany(x => x.ParkingReservations)
                .HasForeignKey(x => x.ParkingSlotId)
                .OnDelete(DeleteBehavior.Restrict);

            // =================================================
            // BOOKING -> PAYMENT
            // =================================================

            modelBuilder.Entity<Payment>()
                .HasOne(x => x.Booking)
                .WithOne(x => x.Payment)
                .HasForeignKey<Payment>(
                    x => x.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // =================================================
            // CUSTOMER -> NOTIFICATION
            // =================================================

            modelBuilder.Entity<Notification>()
                .HasOne(x => x.Customer)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // =================================================
            // ONLY ONE ACTIVE BOOKING PER SEAT
            // =================================================

            modelBuilder.Entity<BookingSeat>()
                .HasIndex(x => x.SeatId)
                .IsUnique()
                .HasFilter("[Status] = 'Active'");

            // =================================================
            // ONLY ONE ACTIVE PARKING RESERVATION
            // =================================================

            modelBuilder.Entity<ParkingReservation>()
                .HasIndex(x => x.ParkingSlotId)
                .IsUnique()
                .HasFilter("[Status] = 'Active'");

            // =================================================
            // DECIMAL PRECISION
            // =================================================

            modelBuilder.Entity<Booking>()
                .Property(x => x.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<BookingSeat>()
                .Property(x => x.SeatPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Event>()
                .Property(x => x.TicketPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Event>()
                .Property(x => x.ParkingFee)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ParkingSlot>()
                .Property(x => x.Fee)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ParkingReservation>()
                .Property(x => x.Fee)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payment>()
                .Property(x => x.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Seat>()
                .Property(x => x.Price)
                .HasPrecision(18, 2);
        }
    }
}