using EventParkingReservation.Data;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservation.Repositories.Implementations
{
    public class NotificationRepository :
        INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Notification>>
            GetByCustomerAsync(int customerId)
        {
            return await _context.Notifications
                .Where(x =>
                    x.CustomerId == customerId)
                .OrderByDescending(x =>
                    x.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Notification?> GetByIdAsync(
            int id)
        {
            return await _context.Notifications
                .FirstOrDefaultAsync(x =>
                    x.NotificationId == id);
        }

        public async Task<int> GetUnreadCountAsync(
            int customerId)
        {
            return await _context.Notifications
                .CountAsync(x =>
                    x.CustomerId == customerId &&
                    !x.IsRead);
        }

        public async Task AddAsync(
            Notification notification)
        {
            _context.Notifications.Add(
                notification);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(
            Notification notification)
        {
            _context.Notifications.Update(
                notification);

            await _context.SaveChangesAsync();
        }
    }
}
