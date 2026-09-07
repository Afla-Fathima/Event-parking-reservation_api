using EventParkingReservation.Data;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservation.Repositories.Implementations
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _db;

        public EventRepository(
            ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<Event>> GetAllAsync(
            string? search,
            DateOnly? date,
            int? venueId,
            int? categoryId)
        {
            var query =
                _db.Events
                    .AsNoTracking()
                    .Include(x => x.Venue)
                    .Include(x => x.Category)
                    .AsQueryable();

            if (!string.IsNullOrWhiteSpace(
                search))
            {
                var value =
                    search.Trim();

                query =
                    query.Where(x =>
                        x.EventName.Contains(value));
            }

            if (date.HasValue)
            {
                var eventDate =
                    date.Value;

                query =
                    query.Where(x =>
                        x.EventDate ==
                            eventDate);
            }

            if (venueId.HasValue)
            {
                var venue =
                    venueId.Value;

                query =
                    query.Where(x =>
                        x.VenueId ==
                            venue);
            }

            if (categoryId.HasValue)
            {
                var category =
                    categoryId.Value;

                query =
                    query.Where(x =>
                        x.CategoryId ==
                            category);
            }

            return await query
                .OrderBy(x =>
                    x.EventDate)
                .ThenBy(x =>
                    x.StartTime)
                .ToListAsync();
        }

        public Task<Event?> GetByIdAsync(
            int id)
        {
            return _db.Events
                .Include(x => x.Venue)
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x =>
                    x.EventId == id);
        }

        public Task<bool> VenueExistsAsync(
            int venueId)
        {
            return _db.Venues
                .AnyAsync(x =>
                    x.VenueId ==
                        venueId);
        }

        public Task<bool> CategoryExistsAsync(
            int categoryId)
        {
            return _db.EventCategories
                .AnyAsync(x =>
                    x.CategoryId ==
                        categoryId);
        }

        public Task<Venue?> GetVenueAsync(
            int venueId)
        {
            return _db.Venues
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.VenueId ==
                        venueId);
        }

        public Task<bool> HasOverlapAsync(
            int venueId,
            DateOnly date,
            TimeOnly start,
            TimeOnly end,
            int? excludeEventId = null)
        {
            return _db.Events
                .AnyAsync(x =>
                    x.VenueId ==
                        venueId &&

                    x.EventDate ==
                        date &&

                    (
                        !excludeEventId.HasValue ||
                        x.EventId !=
                            excludeEventId.Value
                    ) &&

                    start <
                        x.EndTime &&

                    end >
                        x.StartTime);
        }

        public Task<bool>
            HasActiveBookingsAsync(
                int eventId)
        {
            return _db.Bookings
                .AnyAsync(x =>
                    x.EventId ==
                        eventId &&

                    x.Status !=
                        "Cancelled" &&

                    x.Status !=
                        "Expired");
        }

        public Task<int>
            BookedSeatCountAsync(
                int eventId)
        {
            return _db.BookingSeats
                .CountAsync(x =>
                    x.Seat != null &&
                    x.Seat.EventId ==
                        eventId &&
                    x.Status ==
                        "Active");
        }

        public Task<List<int>>
            GetActiveBookingCustomerIdsAsync(
                int eventId)
        {
            return _db.Bookings
                .AsNoTracking()
                .Where(x =>
                    x.EventId ==
                        eventId &&

                    x.Status !=
                        "Cancelled" &&

                    x.Status !=
                        "Expired")
                .Select(x =>
                    x.CustomerId)
                .Distinct()
                .ToListAsync();
        }

        public async Task AddAsync(
            Event entity)
        {
            _db.Events.Add(entity);

            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(
            Event entity)
        {
            _db.Events.Update(entity);

            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(
            Event entity)
        {
            _db.Events.Remove(entity);

            await _db.SaveChangesAsync();
        }
    }
}