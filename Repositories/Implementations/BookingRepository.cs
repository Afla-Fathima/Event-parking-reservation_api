using System.Data;
using EventParkingReservation.Data;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservation.Repositories.Implementations
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking>>
            GetAllAsync(int? eventId = null)
        {
            IQueryable<Booking> query =
                _context.Bookings
                    .Include(x => x.Customer)
                    .Include(x => x.Event)
                    .Include(x => x.BookingSeats)
                        .ThenInclude(x => x.Seat)
                    .Include(x => x.ParkingReservation)
                        .ThenInclude(x => x!.ParkingSlot)
                    .AsNoTracking();

            if (eventId.HasValue)
            {
                query = query.Where(x =>
                    x.EventId == eventId.Value);
            }

            return await query
                .OrderByDescending(x => x.BookingDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>>
            GetByCustomerAsync(int customerId)
        {
            return await _context.Bookings
                .Include(x => x.Customer)
                .Include(x => x.Event)
                .Include(x => x.BookingSeats)
                    .ThenInclude(x => x.Seat)
                .Include(x => x.ParkingReservation)
                    .ThenInclude(x => x!.ParkingSlot)
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.BookingDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(
            int bookingId)
        {
            return await _context.Bookings
                .Include(x => x.Customer)
                .Include(x => x.Event)
                .Include(x => x.BookingSeats)
                    .ThenInclude(x => x.Seat)
                .Include(x => x.ParkingReservation)
                    .ThenInclude(x => x!.ParkingSlot)
                .Include(x => x.Payment)
                .FirstOrDefaultAsync(x =>
                    x.BookingId == bookingId);
        }

        public async Task<Booking>
            CreateReservationAsync(
                Booking booking,
                IEnumerable<int> seatIds,
                int? parkingSlotId)
        {
            await using var transaction =
                await _context.Database
                    .BeginTransactionAsync(
                        IsolationLevel.Serializable);

            try
            {
                List<int> ids =
                    seatIds.Distinct().ToList();

                if (ids.Count == 0)
                {
                    throw new InvalidOperationException(
                        "At least one seat is required.");
                }

                List<Seat> seats =
                    await _context.Seats
                        .Where(x =>
                            ids.Contains(x.SeatId) &&
                            x.EventId == booking.EventId)
                        .ToListAsync();

                if (seats.Count != ids.Count)
                {
                    throw new InvalidOperationException(
                        "One or more selected seats are invalid.");
                }

                bool seatTaken =
                    await _context.BookingSeats
                        .AnyAsync(x =>
                            ids.Contains(x.SeatId) &&
                            x.Status == "Active");

                if (seatTaken)
                {
                    throw new InvalidOperationException(
                        "One or more selected seats were already booked.");
                }

                ParkingSlot? slot = null;

                if (parkingSlotId.HasValue)
                {
                    slot = await _context.ParkingSlots
                        .FirstOrDefaultAsync(x =>
                            x.ParkingSlotId ==
                                parkingSlotId.Value &&
                            x.EventId == booking.EventId);

                    if (slot == null)
                    {
                        throw new InvalidOperationException(
                            "Parking slot was not found.");
                    }

                    bool parkingTaken =
                        await _context.ParkingReservations
                            .AnyAsync(x =>
                                x.ParkingSlotId ==
                                    parkingSlotId.Value &&
                                x.Status == "Active");

                    if (parkingTaken)
                    {
                        throw new InvalidOperationException(
                            "Parking slot is already reserved.");
                    }
                }

                booking.TotalAmount =
                    seats.Sum(x => x.Price);

                if (slot != null)
                {
                    booking.TotalAmount += slot.Fee;
                }

                _context.Bookings.Add(booking);

                await _context.SaveChangesAsync();

                foreach (Seat seat in seats)
                {
                    seat.Status = "Booked";

                    _context.BookingSeats.Add(
                        new BookingSeat
                        {
                            BookingId =
                                booking.BookingId,

                            SeatId =
                                seat.SeatId,

                            SeatPrice =
                                seat.Price,

                            Status =
                                "Active"
                        });
                }

                if (slot != null)
                {
                    slot.Status = "Occupied";

                    _context.ParkingReservations.Add(
                        new ParkingReservation
                        {
                            BookingId =
                                booking.BookingId,

                            ParkingSlotId =
                                slot.ParkingSlotId,

                            Fee =
                                slot.Fee,

                            Status =
                                "Active",

                            ReservedAt =
                                DateTime.UtcNow
                        });
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return booking;
            }
            catch
            {
                await transaction.RollbackAsync();

                throw;
            }
        }

        public async Task AddSeatsAsync(
            int bookingId,
            IEnumerable<int> seatIds)
        {
            Booking booking =
                await _context.Bookings
                    .FirstOrDefaultAsync(x =>
                        x.BookingId == bookingId)
                ?? throw new KeyNotFoundException(
                    "Booking not found.");

            List<int> ids =
                seatIds.Distinct().ToList();

            await using var transaction =
                await _context.Database
                    .BeginTransactionAsync(
                        IsolationLevel.Serializable);

            try
            {
                List<Seat> seats =
                    await _context.Seats
                        .Where(x =>
                            ids.Contains(x.SeatId) &&
                            x.EventId == booking.EventId)
                        .ToListAsync();

                if (seats.Count != ids.Count)
                {
                    throw new InvalidOperationException(
                        "Invalid seat selection.");
                }

                bool taken =
                    await _context.BookingSeats
                        .AnyAsync(x =>
                            ids.Contains(x.SeatId) &&
                            x.Status == "Active");

                if (taken)
                {
                    throw new InvalidOperationException(
                        "One or more seats are already booked.");
                }

                foreach (Seat seat in seats)
                {
                    seat.Status = "Booked";

                    booking.TotalAmount +=
                        seat.Price;

                    _context.BookingSeats.Add(
                        new BookingSeat
                        {
                            BookingId = bookingId,
                            SeatId = seat.SeatId,
                            SeatPrice = seat.Price,
                            Status = "Active"
                        });
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();

                throw;
            }
        }

        public async Task ReserveParkingAsync(
            int bookingId,
            int parkingSlotId)
        {
            Booking booking =
                await _context.Bookings
                    .Include(x =>
                        x.ParkingReservation)
                    .FirstOrDefaultAsync(x =>
                        x.BookingId == bookingId)
                ?? throw new KeyNotFoundException(
                    "Booking not found.");

            if (booking.ParkingReservation != null &&
                booking.ParkingReservation.Status ==
                    "Active")
            {
                throw new InvalidOperationException(
                    "Booking already has parking.");
            }

            await using var transaction =
                await _context.Database
                    .BeginTransactionAsync(
                        IsolationLevel.Serializable);

            try
            {
                ParkingSlot slot =
                    await _context.ParkingSlots
                        .FirstOrDefaultAsync(x =>
                            x.ParkingSlotId ==
                                parkingSlotId &&
                            x.EventId ==
                                booking.EventId)
                    ?? throw new KeyNotFoundException(
                        "Parking slot not found.");

                bool taken =
                    await _context.ParkingReservations
                        .AnyAsync(x =>
                            x.ParkingSlotId ==
                                parkingSlotId &&
                            x.Status == "Active");

                if (taken)
                {
                    throw new InvalidOperationException(
                        "Parking slot is already reserved.");
                }

                slot.Status = "Occupied";

                booking.TotalAmount += slot.Fee;

                _context.ParkingReservations.Add(
                    new ParkingReservation
                    {
                        BookingId = bookingId,
                        ParkingSlotId =
                            parkingSlotId,
                        Fee = slot.Fee,
                        Status = "Active"
                    });

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();

                throw;
            }
        }

        public async Task RemoveParkingAsync(
            int bookingId)
        {
            Booking booking =
                await _context.Bookings
                    .Include(x =>
                        x.ParkingReservation)
                    .ThenInclude(x =>
                        x!.ParkingSlot)
                    .FirstOrDefaultAsync(x =>
                        x.BookingId ==
                            bookingId)
                ?? throw new KeyNotFoundException(
                    "Booking not found.");

            ParkingReservation? reservation =
                booking.ParkingReservation;

            if (reservation == null ||
                reservation.Status != "Active")
            {
                throw new InvalidOperationException(
                    "No active parking reservation.");
            }

            reservation.Status =
                "Released";

            if (reservation.ParkingSlot != null)
            {
                reservation.ParkingSlot.Status =
                    "Available";
            }

            booking.TotalAmount -=
                reservation.Fee;

            if (booking.TotalAmount < 0)
            {
                booking.TotalAmount = 0;
            }

            await _context.SaveChangesAsync();
        }

        public async Task CancelAsync(
            Booking booking)
        {
            booking.Status = "Cancelled";

            foreach (BookingSeat bookingSeat
                     in booking.BookingSeats)
            {
                bookingSeat.Status =
                    "Released";

                if (bookingSeat.Seat != null)
                {
                    bookingSeat.Seat.Status =
                        "Available";
                }
            }

            if (booking.ParkingReservation != null)
            {
                booking.ParkingReservation.Status =
                    "Released";

                if (booking.ParkingReservation
                    .ParkingSlot != null)
                {
                    booking.ParkingReservation
                        .ParkingSlot.Status =
                        "Available";
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(
            Booking booking)
        {
            _context.Bookings.Update(booking);

            await _context.SaveChangesAsync();
        }
    }
}
