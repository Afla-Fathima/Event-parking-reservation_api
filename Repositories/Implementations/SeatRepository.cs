using EventParkingReservation.Data;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservation.Repositories.Implementations
{
    public class SeatRepository : ISeatRepository
    {
        private readonly ApplicationDbContext _db;

        public SeatRepository(
            ApplicationDbContext db)
        {
            _db = db;
        }

        public Task<List<Seat>> GetByEventIdAsync(
            int eventId)
        {
            return _db.Seats
                .AsNoTracking()
                .Where(x =>
                    x.EventId == eventId)
                .OrderBy(x =>
                    x.SeatNumber)
                .ToListAsync();
        }

        public Task<Seat?> GetByIdAsync(
            int seatId)
        {
            return _db.Seats
                .FirstOrDefaultAsync(x =>
                    x.SeatId == seatId);
        }

        public Task<Event?> GetEventAsync(
            int eventId)
        {
            return _db.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.EventId == eventId);
        }

        public Task<int> CountByEventAsync(
            int eventId)
        {
            return _db.Seats.CountAsync(x =>
                x.EventId == eventId);
        }

        public Task<bool> NumberExistsAsync(
            int eventId,
            string number,
            int? excludeId = null)
        {
            return _db.Seats.AnyAsync(x =>
                x.EventId == eventId &&
                x.SeatNumber == number &&
                (
                    !excludeId.HasValue ||
                    x.SeatId !=
                        excludeId.Value
                ));
        }

        public Task<bool> HasActiveBookingAsync(
            int seatId)
        {
            return _db.BookingSeats.AnyAsync(x =>
                x.SeatId == seatId &&
                x.Status == "Active");
        }

        public async Task AddAsync(
            Seat seat)
        {
            _db.Seats.Add(seat);

            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(
            Seat seat)
        {
            _db.Seats.Update(seat);

            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(
            Seat seat)
        {
            _db.Seats.Remove(seat);

            await _db.SaveChangesAsync();
        }
    }
}