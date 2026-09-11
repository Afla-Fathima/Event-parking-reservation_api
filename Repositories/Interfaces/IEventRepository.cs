using EventParkingReservation.Models;

namespace EventParkingReservation.Repositories.Interfaces
{
    public interface IEventRepository
    {
        Task<List<Event>> GetAllAsync(
            string? search,
            DateOnly? date,
            int? venueId,
            int? categoryId);

        Task<Event?> GetByIdAsync(
            int id);

        Task<bool> VenueExistsAsync(
            int venueId);

        Task<bool> CategoryExistsAsync(
            int categoryId);

        Task<Venue?> GetVenueAsync(
            int venueId);

        Task<bool> HasOverlapAsync(
            int venueId,
            DateOnly date,
            TimeOnly start,
            TimeOnly end,
            int? excludeEventId = null);

        // Existing:
        // Used when checking whether an event
        // has CURRENT active bookings.
        Task<bool> HasActiveBookingsAsync(
            int eventId);

        // NEW:
        // Used to lock ticket price once
        // ANY booking history exists.
        Task<bool> HasAnyBookingsAsync(
            int eventId);

        Task<int> BookedSeatCountAsync(
            int eventId);

        Task<List<int>>
            GetActiveBookingCustomerIdsAsync(
                int eventId);

        Task AddAsync(
            Event entity);

        Task UpdateAsync(
            Event entity);

        Task DeleteAsync(
            Event entity);
    }
}