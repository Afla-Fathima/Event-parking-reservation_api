using EventParkingReservation.Data;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservation.Repositories.Implementations
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _context;

        public EventRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Event>> GetAllAsync(
            string? search = null,
            int? venueId = null,
            int? categoryId = null,
            DateOnly? date = null)
        {
            IQueryable<Event> query =
                _context.Events
                    .Include(x => x.Venue)
                    .Include(x => x.Category)
                    .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.EventName.Contains(search));
            }

            if (venueId.HasValue)
            {
                query = query.Where(x =>
                    x.VenueId == venueId.Value);
            }

            if (categoryId.HasValue)
            {
                query = query.Where(x =>
                    x.CategoryId == categoryId.Value);
            }

            if (date.HasValue)
            {
                query = query.Where(x =>
                    x.EventDate == date.Value);
            }

            return await query
                .OrderBy(x => x.EventDate)
                .ThenBy(x => x.StartTime)
                .ToListAsync();
        }

        public async Task<Event?> GetByIdAsync(int id)
        {
            return await _context.Events
                .Include(x => x.Venue)
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.EventId == id);
        }

        public async Task<bool> HasActiveBookingsAsync(
            int eventId)
        {
            return await _context.Bookings.AnyAsync(x =>
                x.EventId == eventId &&
                x.Status != "Cancelled");
        }

        public async Task AddAsync(Event eventEntity)
        {
            _context.Events.Add(eventEntity);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Event eventEntity)
        {
            _context.Events.Update(eventEntity);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Event eventEntity)
        {
            _context.Events.Remove(eventEntity);

            await _context.SaveChangesAsync();
        }
    }
}
