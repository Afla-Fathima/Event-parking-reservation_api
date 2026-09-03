using EventParkingReservation.DTOs.Notification;

namespace EventParkingReservation.Services.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationDto>>
            GetByCustomerAsync(
                int customerId);

        Task<int> GetUnreadCountAsync(
            int customerId);

        Task MarkAsReadAsync(
            int notificationId,
            int customerId);

        Task CreateAsync(
            int customerId,
            string title,
            string message,
            string type);
    }
}