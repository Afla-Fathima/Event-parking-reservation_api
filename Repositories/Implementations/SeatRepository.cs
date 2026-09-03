using EventParkingReservation.Data;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservation.Repositories.Implementations
{
    public class SeatRepository : ISeatRepository
    {
        private readonly ApplicationDbContext _context;

        public SeatRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // GET SEATS BY EVENT
        // ==========================================
        public async Task<IEnumerable<Seat>>
            GetByEventIdAsync(int eventId)
        {
            return await _context.Seats
                .AsNoTracking()
                .Where(s => s.EventId == eventId)
                .OrderBy(s => s.SeatNumber)
                .ToListAsync();
        }

        // ==========================================
        // GET SEAT BY ID
        // ==========================================
        public async Task<Seat?>
            GetByIdAsync(int seatId)
        {
            return await _context.Seats
                .FirstOrDefaultAsync(
                    s => s.SeatId == seatId);
        }

        // ==========================================
        // GET EVENT
        // ==========================================
        public async Task<Event?>
            GetEventByIdAsync(int eventId)
        {
            return await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    e => e.EventId == eventId);
        }

        // ==========================================
        // CHECK DUPLICATE SEAT NUMBER
        // ==========================================
        public async Task<bool>
            SeatNumberExistsAsync(
                int eventId,
                string seatNumber,
                int? excludeSeatId = null)
        {
            string normalizedSeatNumber =
                seatNumber
                    .Trim()
                    .ToUpper();

            var query =
                _context.Seats
                    .Where(s =>
                        s.EventId == eventId &&
                        s.SeatNumber.ToUpper()
                        == normalizedSeatNumber);

            if (excludeSeatId.HasValue)
            {
                query = query.Where(s =>
                    s.SeatId != excludeSeatId.Value);
            }

            return await query.AnyAsync();
        }

        // ==========================================
        // COUNT SEATS
        // ==========================================
        public async Task<int>
            CountByEventAsync(int eventId)
        {
            return await _context.Seats
                .CountAsync(
                    s => s.EventId == eventId);
        }

        // ==========================================
        // CHECK BOOKING
        // ==========================================
        public async Task<bool>
            HasBookingsAsync(int seatId)
        {
            return await _context.BookingSeats
                .AnyAsync(bs =>
                    bs.SeatId == seatId &&
                    bs.Status != "Released");
        }

        // ==========================================
        // ADD
        // ==========================================
        public async Task AddAsync(Seat seat)
        {
            await _context.Seats
                .AddAsync(seat);

            await _context
                .SaveChangesAsync();
        }

        // ==========================================
        // ADD MULTIPLE
        // ==========================================
        public async Task AddRangeAsync(
            IEnumerable<Seat> seats)
        {
            var seatList =
                seats.ToList();

            if (seatList.Count == 0)
            {
                return;
            }

            await _context.Seats
                .AddRangeAsync(seatList);

            await _context
                .SaveChangesAsync();
        }

        // ==========================================
        // UPDATE
        // ==========================================
        public async Task UpdateAsync(
            Seat seat)
        {
            _context.Seats.Update(seat);

            await _context
                .SaveChangesAsync();
        }

        // ==========================================
        // DELETE
        // ==========================================
        public async Task DeleteAsync(
            Seat seat)
        {
            _context.Seats.Remove(seat);

            await _context
                .SaveChangesAsync();
        }
    }
}