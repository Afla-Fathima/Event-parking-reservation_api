using EventParkingReservation.Models;

namespace EventParkingReservation.Repositories.Interfaces
{
    public interface IVenueRepository
    {
        Task<IEnumerable<Venue>> GetAllAsync();
        Task<Venue?> GetByIdAsync(int id);
        Task<bool> HasUpcomingEventsAsync(int venueId);
        Task AddAsync(Venue venue);
        Task UpdateAsync(Venue venue);
        Task DeleteAsync(Venue venue);
    }
}
