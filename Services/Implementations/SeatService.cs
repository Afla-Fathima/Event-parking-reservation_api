using EventParkingReservation.Data;
using EventParkingReservation.DTOs.Seat;
using EventParkingReservation.Models;
using EventParkingReservation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservation.Services
{
    public class SeatService : ISeatService
    {
        private readonly ApplicationDbContext _context;

        public SeatService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SeatDto>> GetByEventIdAsync(
            int eventId)
        {
            var eventExists =
                await _context.Events
                    .AnyAsync(e => e.EventId == eventId);

            if (!eventExists)
            {
                throw new KeyNotFoundException(
                    "Event not found."
                );
            }

            return await _context.Seats
                .Where(s => s.EventId == eventId)
                .OrderBy(s => s.SeatNumber)
                .Select(s => new SeatDto
                {
                    SeatId = s.SeatId,
                    EventId = s.EventId,
                    SeatNumber = s.SeatNumber,
                    SeatType = s.SeatType,
                    Price = s.Price,
                    Status = s.Status
                })
                .ToListAsync();
        }

        public async Task<SeatDto> CreateAsync(
            int eventId,
            CreateSeatDto dto)
        {
            var eventItem =
                await _context.Events
                    .FirstOrDefaultAsync(
                        e => e.EventId == eventId
                    );

            if (eventItem == null)
            {
                throw new KeyNotFoundException(
                    "Event not found."
                );
            }

            var seatNumber =
                dto.SeatNumber
                    .Trim()
                    .ToUpper();

            var duplicate =
                await _context.Seats
                    .AnyAsync(s =>
                        s.EventId == eventId &&
                        s.SeatNumber == seatNumber
                    );

            if (duplicate)
            {
                throw new InvalidOperationException(
                    $"Seat {seatNumber} already exists."
                );
            }

            var seat = new Seat
            {
                EventId = eventId,
                SeatNumber = seatNumber,
                SeatType = dto.SeatType.Trim(),
                Price = dto.Price,
                Status = "Available"
            };

            _context.Seats.Add(seat);

            await _context.SaveChangesAsync();

            return ToDto(seat);
        }

        public async Task<SeatDto> UpdateAsync(
            int eventId,
            int seatId,
            UpdateSeatDto dto)
        {
            var seat =
                await _context.Seats
                    .FirstOrDefaultAsync(s =>
                        s.SeatId == seatId &&
                        s.EventId == eventId
                    );

            if (seat == null)
            {
                throw new KeyNotFoundException(
                    "Seat not found."
                );
            }

            var seatNumber =
                dto.SeatNumber
                    .Trim()
                    .ToUpper();

            var duplicate =
                await _context.Seats
                    .AnyAsync(s =>
                        s.EventId == eventId &&
                        s.SeatNumber == seatNumber &&
                        s.SeatId != seatId
                    );

            if (duplicate)
            {
                throw new InvalidOperationException(
                    $"Seat {seatNumber} already exists."
                );
            }

            seat.SeatNumber = seatNumber;
            seat.SeatType = dto.SeatType.Trim();
            seat.Price = dto.Price;
            seat.Status = dto.Status.Trim();

            await _context.SaveChangesAsync();

            return ToDto(seat);
        }

        public async Task DeleteAsync(
            int eventId,
            int seatId)
        {
            var seat =
                await _context.Seats
                    .Include(s => s.BookingSeats)
                    .FirstOrDefaultAsync(s =>
                        s.SeatId == seatId &&
                        s.EventId == eventId
                    );

            if (seat == null)
            {
                throw new KeyNotFoundException(
                    "Seat not found."
                );
            }

            if (seat.BookingSeats.Any())
            {
                throw new InvalidOperationException(
                    "This seat has booking records and cannot be deleted."
                );
            }

            _context.Seats.Remove(seat);

            await _context.SaveChangesAsync();
        }

        public async Task<int> GenerateDefaultSeatsAsync(
            int eventId)
        {
            var eventItem =
                await _context.Events
                    .FirstOrDefaultAsync(
                        e => e.EventId == eventId
                    );

            if (eventItem == null)
            {
                throw new KeyNotFoundException(
                    "Event not found."
                );
            }

            var existingSeatNumbers =
                await _context.Seats
                    .Where(s => s.EventId == eventId)
                    .Select(s => s.SeatNumber)
                    .ToListAsync();

            var existing =
                existingSeatNumbers
                    .Select(x =>
                        x.Trim().ToUpper()
                    )
                    .ToHashSet();

            var newSeats =
                new List<Seat>();

            // A - T = 20 rows
            for (char row = 'A';
                 row <= 'T';
                 row++)
            {
                // 10 seats per row
                for (int number = 1;
                     number <= 10;
                     number++)
                {
                    // A01, A02 ... T10
                    string seatNumber =
                        $"{row}{number:00}";

                    if (existing.Contains(
                        seatNumber))
                    {
                        continue;
                    }

                    var seat =
                        new Seat
                        {
                            EventId = eventId,

                            SeatNumber =
                                seatNumber,

                            SeatType =
                                "Regular",

                            Price =
                                eventItem.TicketPrice,

                            Status =
                                "Available"
                        };

                    newSeats.Add(seat);
                }
            }

            if (newSeats.Count == 0)
            {
                throw new InvalidOperationException(
                    "All 200 seats are already configured for this event."
                );
            }

            await _context.Seats
                .AddRangeAsync(newSeats);

            await _context
                .SaveChangesAsync();

            return newSeats.Count;
        }

        private static SeatDto ToDto(
            Seat seat)
        {
            return new SeatDto
            {
                SeatId = seat.SeatId,
                EventId = seat.EventId,
                SeatNumber = seat.SeatNumber,
                SeatType = seat.SeatType,
                Price = seat.Price,
                Status = seat.Status
            };
        }
    }
}