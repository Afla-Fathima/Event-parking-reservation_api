using EventParkingReservation.Models;

namespace EventParkingReservation.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task<IEnumerable<Notification>> GetByCustomerAsync(int customerId);

        Task<Notification?> GetByIdAsync(int id);

        Task<int> GetUnreadCountAsync(int customerId);

        Task AddAsync(Notification notification);

        Task UpdateAsync(Notification notification);
    }
}
