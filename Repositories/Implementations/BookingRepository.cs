using System.Data;
using EventParkingReservation.Data;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservation.Repositories.Implementations
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _db;

        public BookingRepository(ApplicationDbContext db)
        {
            _db = db;
        }


        public Task<bool> CustomerExistsAsync(int customerId)
        {
            return _db.Customers.AnyAsync(x =>
                x.CustomerId == customerId &&
                x.Status == "Active" &&
                x.Role == "Customer");
        }



        public Task<Event?> GetEventAsync(int eventId)
        {
            return _db.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.EventId == eventId);
        }



        public async Task<Booking> CreateReservationAsync(
     Booking booking,
     IReadOnlyCollection<int> seatIds,
     int? parkingSlotId)
        {
            Booking? result = null;

            var strategy =
                _db.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction =
                    await _db.Database.BeginTransactionAsync(
                        IsolationLevel.Serializable);

                try
                {
                    // ================================
                    // 1. Validate seats
                    // ================================

                    var requestedSeatIds =
                        seatIds
                        .Distinct()
                        .ToList();


                    if (requestedSeatIds.Count == 0)
                    {
                        throw new InvalidOperationException(
                            "A booking must contain at least one seat.");
                    }


                    var seats =
                        await _db.Seats
                        .Where(x =>
                            requestedSeatIds.Contains(x.SeatId)
                            &&
                            x.EventId == booking.EventId)
                        .ToListAsync();


                    if (seats.Count != requestedSeatIds.Count)
                    {
                        throw new InvalidOperationException(
                            "Invalid seat selection.");
                    }


                    decimal totalAmount = 0;


                    foreach (var seat in seats)
                    {
                        if (seat.Status.ToLower() != "available")
                        {
                            throw new InvalidOperationException(
                                $"Seat {seat.SeatNumber} is not available.");
                        }

                        totalAmount += seat.Price;
                    }


                    // ================================
                    // 2. Validate parking
                    // ================================

                    ParkingSlot? parking = null;


                    if (parkingSlotId.HasValue)
                    {
                        parking =
                            await _db.ParkingSlots
                            .FirstOrDefaultAsync(x =>
                                x.ParkingSlotId == parkingSlotId.Value
                                &&
                                x.EventId == booking.EventId);


                        if (parking == null)
                        {
                            throw new InvalidOperationException(
                                "Parking slot not found.");
                        }


                        if (parking.Status.ToLower() != "available")
                        {
                            throw new InvalidOperationException(
                                "Parking slot already reserved.");
                        }


                        totalAmount += parking.Fee;
                    }



                    // ================================
                    // 3. FIRST CREATE BOOKING
                    // ================================

                    booking.TotalAmount = totalAmount;

                    await _db.Bookings.AddAsync(booking);

                    await _db.SaveChangesAsync();


                    // NOW booking.BookingId generated


                    // ================================
                    // 4. Add seats
                    // ================================

                    foreach (var seat in seats)
                    {

                        seat.Status = "Reserved";


                        _db.BookingSeats.Add(
                            new BookingSeat
                            {
                                BookingId = booking.BookingId,

                                SeatId = seat.SeatId,

                                SeatPrice = seat.Price,

                                Status = "Active"
                            });
                    }



                    // ================================
                    // 5. Add parking
                    // ================================

                    if (parking != null)
                    {

                        parking.Status = "Occupied";


                        _db.ParkingReservations.Add(
                            new ParkingReservation
                            {
                                BookingId = booking.BookingId,

                                ParkingSlotId = parking.ParkingSlotId,

                                Fee = parking.Fee,

                                Status = "Active",

                                ReservedAt = DateTime.UtcNow
                            });
                    }



                    // ================================
                    // 6. Final Save
                    // ================================

                    await _db.SaveChangesAsync();


                    await transaction.CommitAsync();


                    result = booking;

                }
                catch
                {
                    await transaction.RollbackAsync();

                    throw;
                }

            });


            return result!;
        }
        public async Task<Booking> AddSeatsAsync(
    int bookingId,
    IReadOnlyCollection<int> seatIds)
        {

            Booking? result = null;


            await ExecuteTransactionAsync(async () =>
            {

                var booking =
                    await _db.Bookings
                    .FirstOrDefaultAsync(x =>
                        x.BookingId == bookingId)
                    ?? throw new KeyNotFoundException(
                        "Booking not found.");



                EnsureEditable(booking);



                var requestedSeatIds =
                    seatIds
                    .Distinct()
                    .ToList();



                if (requestedSeatIds.Count == 0)
                {
                    throw new InvalidOperationException(
                        "At least one seat is required.");
                }



                var seats =
                    await _db.Seats
                    .Where(x =>
                        requestedSeatIds.Contains(x.SeatId)
                        &&
                        x.EventId == booking.EventId)
                    .ToListAsync();



                if (seats.Count != requestedSeatIds.Count)
                {
                    throw new InvalidOperationException(
                        "Invalid seat selection.");
                }




                foreach (var seat in seats)
                {

                    if (seat.Status != "Available")
                    {
                        throw new InvalidOperationException(
                            $"Seat {seat.SeatNumber} is not available.");
                    }



                    seat.Status =
                        "Booked";



                    booking.TotalAmount +=
                        seat.Price;



                    _db.BookingSeats.Add(
                        new BookingSeat
                        {
                            BookingId =
                                bookingId,

                            SeatId =
                                seat.SeatId,

                            SeatPrice =
                                seat.Price,

                            Status =
                                "Active"
                        });

                }




                await _db.SaveChangesAsync();



                result =
                    await GetByIdAsync(bookingId);


            });



            return result!;
        }






        public async Task<Booking> ReserveParkingAsync(
            int bookingId,
            int parkingSlotId)
        {

            Booking? result = null;



            await ExecuteTransactionAsync(async () =>
            {

                var booking =
                    await _db.Bookings

                    .Include(x =>
                        x.ParkingReservation)

                    .ThenInclude(x =>
                        x!.ParkingSlot)

                    .FirstOrDefaultAsync(x =>
                        x.BookingId == bookingId)

                    ?? throw new KeyNotFoundException(
                        "Booking not found.");



                EnsureEditable(booking);




                var slot =
                    await _db.ParkingSlots

                    .FirstOrDefaultAsync(x =>
                        x.ParkingSlotId ==
                            parkingSlotId
                        &&
                        x.EventId ==
                            booking.EventId);



                if (slot == null)
                {
                    throw new InvalidOperationException(
                        "Parking slot not found.");
                }




                if (slot.Status != "Available")
                {
                    throw new InvalidOperationException(
                        "Parking slot already occupied.");
                }




                slot.Status =
                    "Occupied";





                if (booking.ParkingReservation == null)
                {

                    _db.ParkingReservations.Add(
                        new ParkingReservation
                        {
                            BookingId =
                                bookingId,

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
                else
                {

                    booking.ParkingReservation
                        .ParkingSlotId =
                            slot.ParkingSlotId;


                    booking.ParkingReservation
                        .Fee =
                            slot.Fee;


                    booking.ParkingReservation
                        .Status =
                            "Active";


                    booking.ParkingReservation
                        .ReservedAt =
                            DateTime.UtcNow;

                }





                booking.TotalAmount +=
                    slot.Fee;




                await _db.SaveChangesAsync();



                result =
                    await GetByIdAsync(bookingId);



            });



            return result!;
        }






        public async Task<Booking> RemoveParkingAsync(
            int bookingId)
        {

            Booking? result = null;



            await ExecuteTransactionAsync(async () =>
            {

                var booking =
                    await _db.Bookings

                    .Include(x =>
                        x.ParkingReservation)

                    .ThenInclude(x =>
                        x!.ParkingSlot)

                    .FirstOrDefaultAsync(x =>
                        x.BookingId == bookingId)

                    ?? throw new KeyNotFoundException(
                        "Booking not found.");




                EnsureEditable(booking);




                var reservation =
                    booking.ParkingReservation;



                if (reservation == null)
                {
                    throw new InvalidOperationException(
                        "No parking reservation found.");
                }





                reservation.Status =
                    "Released";




                if (reservation.ParkingSlot != null)
                {

                    reservation
                        .ParkingSlot
                        .Status =
                            "Available";

                }




                booking.TotalAmount =
                    Math.Max(
                        0m,
                        booking.TotalAmount -
                        reservation.Fee);





                await _db.SaveChangesAsync();



                result =
                    await GetByIdAsync(bookingId);



            });



            return result!;
        }
        public Task<Booking?> GetByIdAsync(
    int id)
        {

            return _db.Bookings

                .Include(x => x.Customer)

                .Include(x => x.Event)

                .Include(x => x.BookingSeats)
                    .ThenInclude(x => x.Seat)

                .Include(x => x.ParkingReservation)
                    .ThenInclude(x => x!.ParkingSlot)

                .Include(x => x.Payment)

                .FirstOrDefaultAsync(x =>
                    x.BookingId == id);

        }





        public Task<List<Booking>> GetByCustomerAsync(
            int customerId)
        {

            return _db.Bookings

                .AsNoTracking()

                .Include(x => x.Customer)

                .Include(x => x.Event)

                .Include(x => x.BookingSeats)
                    .ThenInclude(x => x.Seat)

                .Include(x => x.ParkingReservation)
                    .ThenInclude(x => x!.ParkingSlot)

                .Where(x =>
                    x.CustomerId == customerId)

                .OrderByDescending(x =>
                    x.BookingDate)

                .ToListAsync();

        }






        public Task<List<Booking>> GetByEventAsync(
            int eventId)
        {

            return _db.Bookings

                .AsNoTracking()

                .Include(x => x.Customer)

                .Include(x => x.Event)

                .Include(x => x.BookingSeats)
                    .ThenInclude(x => x.Seat)

                .Include(x => x.ParkingReservation)
                    .ThenInclude(x => x!.ParkingSlot)

                .Where(x =>
                    x.EventId == eventId)

                .OrderByDescending(x =>
                    x.BookingDate)

                .ToListAsync();

        }






        public async Task CancelAsync(
            Booking booking)
        {

            await ExecuteTransactionAsync(async () =>
            {

                var existing =
                    await _db.Bookings

                    .Include(x =>
                        x.BookingSeats)

                    .ThenInclude(x =>
                        x.Seat)

                    .Include(x =>
                        x.ParkingReservation)

                    .ThenInclude(x =>
                        x!.ParkingSlot)

                    .FirstOrDefaultAsync(x =>
                        x.BookingId ==
                        booking.BookingId);



                if (existing == null)
                {
                    throw new KeyNotFoundException(
                        "Booking not found.");
                }




                if (existing.Status ==
                    "Cancelled")
                {
                    return;
                }




                existing.Status =
                    "Cancelled";





                foreach (var bookingSeat
                    in existing.BookingSeats)
                {

                    bookingSeat.Status =
                        "Released";



                    if (bookingSeat.Seat != null)
                    {

                        bookingSeat.Seat.Status =
                            "Available";

                    }

                }






                if (existing.ParkingReservation != null)
                {

                    existing
                    .ParkingReservation
                    .Status =
                        "Released";



                    if (existing
                        .ParkingReservation
                        .ParkingSlot != null)
                    {

                        existing
                        .ParkingReservation
                        .ParkingSlot
                        .Status =
                            "Available";

                    }

                }





                await _db.SaveChangesAsync();


            });

        }






        private async Task ExecuteTransactionAsync(
            Func<Task> action)
        {

            var strategy =
                _db.Database.CreateExecutionStrategy();
            await _db.Database.BeginTransactionAsync();


            await strategy.ExecuteAsync(async () =>
            {

                await using var transaction =
                    await _db.Database.BeginTransactionAsync(
                        IsolationLevel.Serializable);



                try
                {

                    await action();


                    await transaction.CommitAsync();

                }
                catch
                {

                    await transaction.RollbackAsync();

                    throw;

                }

            });

        }






        private static void EnsureEditable(
            Booking booking)
        {

            if (booking.Status != "Pending")
            {
                throw new InvalidOperationException(
                    "Only pending bookings can be modified.");
            }




            if (booking.HoldExpiresAt.HasValue &&
               booking.HoldExpiresAt.Value <=
               DateTime.UtcNow)
            {

                throw new InvalidOperationException(
                    "Booking hold has expired.");

            }

        }


    }
}
  
