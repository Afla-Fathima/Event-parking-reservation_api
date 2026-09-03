using EventParkingReservation.Data;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservation.Repositories.Implementations
{
    public class VenueRepository : IVenueRepository
    {
        private readonly ApplicationDbContext _context;

        public VenueRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Venue>> GetAllAsync()
        {
            return await _context.Venues
                .AsNoTracking()
                .OrderBy(x => x.VenueName)
                .ToListAsync();
        }

        public async Task<Venue?> GetByIdAsync(int id)
        {
            return await _context.Venues
                .FirstOrDefaultAsync(x => x.VenueId == id);
        }

        public async Task<bool> HasUpcomingEventsAsync(int venueId)
        {
            DateOnly today =
                DateOnly.FromDateTime(DateTime.Today);

            return await _context.Events.AnyAsync(x =>
                x.VenueId == venueId &&
                x.EventDate >= today);
        }

        public async Task AddAsync(Venue venue)
        {
            _context.Venues.Add(venue);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Venue venue)
        {
            _context.Venues.Update(venue);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Venue venue)
        {
            _context.Venues.Remove(venue);

            await _context.SaveChangesAsync();
        }
    }
}
