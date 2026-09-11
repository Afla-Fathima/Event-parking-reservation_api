using EventParkingReservation.Data;
using EventParkingReservation.DTOs.Seat;
using EventParkingReservation.Models;
using EventParkingReservation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace EventParkingReservation.Services.Implementations
{
    public class SeatService : ISeatService
    {
        private readonly ApplicationDbContext _context;

        public SeatService(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // GET ALL SEATS FOR EVENT
        // =====================================================

        public async Task<List<SeatDto>>
            GetByEventIdAsync(
                int eventId)
        {
            var eventExists =
                await _context.Events
                    .AnyAsync(
                        e => e.EventId ==
                             eventId);

            if (!eventExists)
            {
                throw new KeyNotFoundException(
                    "Event not found.");
            }

            var seats =
                await _context.Seats
                    .AsNoTracking()
                    .Where(
                        s => s.EventId ==
                             eventId)
                    .ToListAsync();

            // Correct sorting:
            // A01 A02 ... A10
            // B01 B02 ... B10
            // ...
            // T10
            return seats
                .OrderBy(
                    s =>
                        GetSeatSortKey(
                            s.SeatNumber).Row)
                .ThenBy(
                    s =>
                        GetSeatSortKey(
                            s.SeatNumber).Number)
                .ThenBy(
                    s =>
                        s.SeatNumber)
                .Select(ToDto)
                .ToList();
        }

        // =====================================================
        // CREATE ONE SEAT
        // =====================================================

        public async Task<SeatDto>
            CreateAsync(
                int eventId,
                CreateSeatDto dto)
        {
            var eventItem =
                await _context.Events
                    .FirstOrDefaultAsync(
                        e =>
                            e.EventId ==
                            eventId);

            if (eventItem == null)
            {
                throw new KeyNotFoundException(
                    "Event not found.");
            }

            // A1 => A01
            // A-1 => A01
            // A 1 => A01
            var seatNumber =
                NormalizeSeatNumber(
                    dto.SeatNumber);

            var existingNumbers =
                await _context.Seats
                    .Where(
                        s =>
                            s.EventId ==
                            eventId)
                    .Select(
                        s =>
                            s.SeatNumber)
                    .ToListAsync();

            // Treat A1 and A01
            // as same seat.
            var duplicate =
                existingNumbers.Any(
                    existing =>
                        TryNormalizeSeatNumber(
                            existing,
                            out var normalized) &&
                        normalized ==
                            seatNumber);

            if (duplicate)
            {
                throw new InvalidOperationException(
                    $"Seat {seatNumber} already exists.");
            }

            var seat =
                new Seat
                {
                    EventId =
                        eventId,

                    SeatNumber =
                        seatNumber,

                    SeatType =
                        string.IsNullOrWhiteSpace(
                            dto.SeatType)
                            ? "Regular"
                            : dto.SeatType.Trim(),

                    Price =
                        dto.Price,

                    Status =
                        "Available"
                };

            _context.Seats.Add(
                seat);

            await _context
                .SaveChangesAsync();

            return ToDto(
                seat);
        }

        // =====================================================
        // UPDATE ONE SEAT
        // =====================================================

        public async Task<SeatDto>
            UpdateAsync(
                int eventId,
                int seatId,
                UpdateSeatDto dto)
        {
            var seat =
                await _context.Seats
                    .Include(
                        s =>
                            s.BookingSeats)
                    .FirstOrDefaultAsync(
                        s =>
                            s.EventId ==
                                eventId &&
                            s.SeatId ==
                                seatId);

            if (seat == null)
            {
                throw new KeyNotFoundException(
                    "Seat not found.");
            }

            // Don't modify currently booked seat
            if (
                seat.BookingSeats.Any(
                    x =>
                        x.Status ==
                        "Active"))
            {
                throw new InvalidOperationException(
                    "Booked/reserved seat cannot be modified.");
            }

            var seatNumber =
                NormalizeSeatNumber(
                    dto.SeatNumber);

            var otherSeatNumbers =
                await _context.Seats
                    .Where(
                        s =>
                            s.EventId ==
                                eventId &&
                            s.SeatId !=
                                seatId)
                    .Select(
                        s =>
                            s.SeatNumber)
                    .ToListAsync();

            var duplicate =
                otherSeatNumbers.Any(
                    existing =>
                        TryNormalizeSeatNumber(
                            existing,
                            out var normalized) &&
                        normalized ==
                            seatNumber);

            if (duplicate)
            {
                throw new InvalidOperationException(
                    $"Seat {seatNumber} already exists.");
            }

            seat.SeatNumber =
                seatNumber;

            seat.SeatType =
                string.IsNullOrWhiteSpace(
                    dto.SeatType)
                    ? "Regular"
                    : dto.SeatType.Trim();

            seat.Price =
                dto.Price;

            seat.Status =
                string.IsNullOrWhiteSpace(
                    dto.Status)
                    ? "Available"
                    : dto.Status.Trim();

            await _context
                .SaveChangesAsync();

            return ToDto(
                seat);
        }

        // =====================================================
        // DELETE ONE SEAT
        // =====================================================

        public async Task DeleteAsync(
            int eventId,
            int seatId)
        {
            var seat =
                await _context.Seats
                    .Include(
                        s =>
                            s.BookingSeats)
                    .FirstOrDefaultAsync(
                        s =>
                            s.EventId ==
                                eventId &&
                            s.SeatId ==
                                seatId);

            if (seat == null)
            {
                throw new KeyNotFoundException(
                    "Seat not found.");
            }

            if (
                seat.BookingSeats.Any())
            {
                throw new InvalidOperationException(
                    "Seat has booking history and cannot be deleted.");
            }

            _context.Seats.Remove(
                seat);

            await _context
                .SaveChangesAsync();
        }

        // =====================================================
        // GENERATE 200 SEATS
        // =====================================================
        //
        // A01 A02 A03 ... A10
        // B01 B02 B03 ... B10
        // ...
        // T01 T02 T03 ... T10
        //
        // A-T = 20 rows
        // 10 seats each
        //
        // 20 * 10 = 200 SEATS
        //
        // =====================================================

        public async Task<int>
            GenerateDefaultSeatsAsync(
                int eventId)
        {
            // ---------------------------------------------
            // CHECK EVENT
            // ---------------------------------------------

            var eventItem =
                await _context.Events
                    .FirstOrDefaultAsync(
                        e =>
                            e.EventId ==
                            eventId);

            if (eventItem == null)
            {
                throw new KeyNotFoundException(
                    "Event not found.");
            }

            // ---------------------------------------------
            // GET EXISTING SEATS
            // ---------------------------------------------

            var existingSeatNumbers =
                await _context.Seats
                    .Where(
                        s =>
                            s.EventId ==
                            eventId)
                    .Select(
                        s =>
                            s.SeatNumber)
                    .ToListAsync();

            // Important:
            //
            // If database already has:
            // A1
            //
            // we consider it same as:
            // A01
            //
            // so duplicate A01 will NOT be created.

            var existingCanonicalSeats =
                new HashSet<string>(
                    StringComparer
                        .OrdinalIgnoreCase);

            foreach (
                var existingSeatNumber
                in existingSeatNumbers)
            {
                if (
                    TryNormalizeSeatNumber(
                        existingSeatNumber,
                        out var normalized))
                {
                    existingCanonicalSeats
                        .Add(
                            normalized);
                }
            }

            // ---------------------------------------------
            // PREPARE NEW SEATS
            // ---------------------------------------------

            var newSeats =
                new List<Seat>();

            // Row A through T
            for (
                char row = 'A';
                row <= 'T';
                row++)
            {
                // Seat 1 through 10
                for (
                    int number = 1;
                    number <= 10;
                    number++)
                {
                    // 1 -> 01
                    // 2 -> 02
                    // 10 -> 10

                    var seatNumber =
                        $"{row}{number:00}";

                    // Example:
                    //
                    // A01
                    // A02
                    // ...
                    // A10
                    //
                    // B01...
                    //
                    // T10

                    if (
                        existingCanonicalSeats
                            .Contains(
                                seatNumber))
                    {
                        continue;
                    }

                    var seat =
                        new Seat
                        {
                            EventId =
                                eventId,

                            SeatNumber =
                                seatNumber,

                            SeatType =
                                "Regular",

                            // Seat price taken
                            // from Event ticket price
                            Price =
                                eventItem
                                    .TicketPrice,

                            Status =
                                "Available"
                        };

                    newSeats.Add(
                        seat);
                }
            }

            // ---------------------------------------------
            // NOTHING TO CREATE
            // ---------------------------------------------

            if (
                newSeats.Count ==
                0)
            {
                throw new InvalidOperationException(
                    "The complete 200-seat map is already configured for this event.");
            }

            // ---------------------------------------------
            // INSERT ALL AT ONCE
            // ---------------------------------------------

            await _context.Seats
                .AddRangeAsync(
                    newSeats);

            await _context
                .SaveChangesAsync();

            return newSeats.Count;
        }

        // =====================================================
        // DTO MAPPING
        // =====================================================

        private static SeatDto ToDto(
            Seat seat)
        {
            return new SeatDto
            {
                SeatId =
                    seat.SeatId,

                EventId =
                    seat.EventId,

                SeatNumber =
                    seat.SeatNumber,

                SeatType =
                    seat.SeatType,

                Price =
                    seat.Price,

                Status =
                    seat.Status
            };
        }

        // =====================================================
        // NORMALIZE
        // =====================================================
        //
        // A1 -> A01
        // A01 -> A01
        // A-1 -> A01
        // A 1 -> A01
        //
        // =====================================================

        private static string
            NormalizeSeatNumber(
                string value)
        {
            if (
                !TryNormalizeSeatNumber(
                    value,
                    out var normalized))
            {
                throw new InvalidOperationException(
                    "Seat number must be between A01 and T10. Example: A01, B05, T10.");
            }

            return normalized;
        }

        // =====================================================
        // TRY NORMALIZE
        // =====================================================

        private static bool
            TryNormalizeSeatNumber(
                string? value,
                out string normalized)
        {
            normalized =
                string.Empty;

            if (
                string.IsNullOrWhiteSpace(
                    value))
            {
                return false;
            }

            var clean =
                value
                    .Trim()
                    .ToUpperInvariant()
                    .Replace(
                        "-",
                        string.Empty)
                    .Replace(
                        " ",
                        string.Empty);

            // A-T
            // 01-10
            var match =
                Regex.Match(
                    clean,
                    @"^([A-T])(0?[1-9]|10)$");

            if (
                !match.Success)
            {
                return false;
            }

            var row =
                match.Groups[1]
                    .Value[0];

            var number =
                int.Parse(
                    match.Groups[2]
                        .Value);

            normalized =
                $"{row}{number:00}";

            return true;
        }

        // =====================================================
        // SORT KEY
        // =====================================================

        private static
            (int Row, int Number)
            GetSeatSortKey(
                string value)
        {
            if (
                TryNormalizeSeatNumber(
                    value,
                    out var normalized))
            {
                return
                (
                    normalized[0] - 'A',

                    int.Parse(
                        normalized[1..])
                );
            }

            return
            (
                int.MaxValue,
                int.MaxValue
            );
        }
    }
}