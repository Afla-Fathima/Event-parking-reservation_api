using EventParkingReservation.DTOs.Notification;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Interfaces;
using EventParkingReservation.Services.Interfaces;

namespace EventParkingReservation.Services.Implementations
{
    public class NotificationService :
        INotificationService
    {
        private readonly INotificationRepository
            _repository;

        public NotificationService(
            INotificationRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<NotificationDto>>
            GetByCustomerAsync(
                int customerId)
        {
            var notifications =
                await _repository
                    .GetByCustomerAsync(
                        customerId);

            return notifications.Select(Map);
        }

        public async Task<int>
            GetUnreadCountAsync(
                int customerId)
        {
            return await _repository
                .GetUnreadCountAsync(
                    customerId);
        }

        public async Task MarkAsReadAsync(
            int notificationId,
            int customerId)
        {
            var notification =
                await _repository
                    .GetByIdAsync(
                        notificationId)
                ?? throw new KeyNotFoundException(
                    "Notification not found.");

            if (notification.CustomerId !=
                customerId)
            {
                throw new UnauthorizedAccessException(
                    "You cannot access this notification.");
            }

            notification.IsRead = true;

            await _repository.UpdateAsync(
                notification);
        }

        public async Task CreateAsync(
            int customerId,
            string title,
            string message,
            string type)
        {
            var notification =
                new Notification
                {
                    CustomerId =
                        customerId,

                    Title =
                        title,

                    Message =
                        message,

                    Type =
                        type,

                    IsRead =
                        false,

                    CreatedAt =
                        DateTime.UtcNow
                };

            await _repository.AddAsync(
                notification);
        }

        private static NotificationDto Map(
            Notification notification)
        {
            return new NotificationDto
            {
                NotificationId =
                    notification.NotificationId,

                CustomerId =
                    notification.CustomerId,

                Title =
                    notification.Title,

                Message =
                    notification.Message,

                Type =
                    notification.Type,

                IsRead =
                    notification.IsRead,

                CreatedAt =
                    notification.CreatedAt
            };
        }
    }
}