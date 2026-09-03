using EventParkingReservation.Models;

namespace EventParkingReservation.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<EventCategory>> GetAllAsync();

        Task<EventCategory?> GetByIdAsync(int id);

        Task<bool> NameExistsAsync(
            string name,
            int? excludeId = null);

        Task AddAsync(EventCategory category);

        Task UpdateAsync(EventCategory category);

        Task DeleteAsync(EventCategory category);
    }
}