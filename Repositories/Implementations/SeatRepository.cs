using EventParkingReservation.Data;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservation.Repositories.Implementations
{
    public class SeatRepository : ISeatRepository
    {
        private readonly ApplicationDbContext _context;

        public SeatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Seat>> GetByEventIdAsync(
            int eventId)
        {
            return await _context.Seats
                .Where(x => x.EventId == eventId)
                .OrderBy(x => x.SeatNumber)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Seat?> GetByIdAsync(int seatId)
        {
            return await _context.Seats
                .FirstOrDefaultAsync(x =>
                    x.SeatId == seatId);
        }

        public async Task<bool> SeatNumberExistsAsync(
            int eventId,
            string seatNumber,
            int? excludeSeatId = null)
        {
            return await _context.Seats.AnyAsync(x =>
                x.EventId == eventId &&
                x.SeatNumber == seatNumber &&
                (!excludeSeatId.HasValue ||
                 x.SeatId != excludeSeatId.Value));
        }

        public async Task<bool> HasActiveBookingAsync(
            int seatId)
        {
            return await _context.BookingSeats.AnyAsync(x =>
                x.SeatId == seatId &&
                x.Status == "Active");
        }

        public async Task AddAsync(Seat seat)
        {
            _context.Seats.Add(seat);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Seat seat)
        {
            _context.Seats.Update(seat);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Seat seat)
        {
            _context.Seats.Remove(seat);

            await _context.SaveChangesAsync();
        }
    }
}
