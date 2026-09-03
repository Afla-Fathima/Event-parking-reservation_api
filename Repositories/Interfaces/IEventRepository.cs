using EventParkingReservation.Models;

namespace EventParkingReservation.Repositories.Interfaces
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetAllAsync(
            string? search = null,
            int? venueId = null,
            int? categoryId = null,
            DateOnly? date = null);

        Task<Event?> GetByIdAsync(int id);

        Task<bool> HasActiveBookingsAsync(int eventId);

        Task AddAsync(Event eventEntity);

        Task UpdateAsync(Event eventEntity);

        Task DeleteAsync(Event eventEntity);
    }
}