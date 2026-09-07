using EventParkingReservation.Data;
using EventParkingReservation.DTOs.Dashboard;
using EventParkingReservation.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservation.Repositories.Implementations
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _db;

        public DashboardRepository(
            ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<CustomerDashboardDto>
            GetCustomerAsync(
                int customerId)
        {
            var today =
                DateOnly.FromDateTime(
                    DateTime.Today);

            var upcomingBookings =
                await _db.Bookings
                    .CountAsync(x =>
                        x.CustomerId == customerId &&
                        x.Status != "Cancelled" &&
                        x.Status != "Expired" &&
                        x.Event != null &&
                        x.Event.EventDate >= today);

            var reservedParking =
                await _db.ParkingReservations
                    .CountAsync(x =>
                        x.Booking != null &&
                        x.Booking.CustomerId ==
                            customerId &&
                        x.Status == "Active");

            var recentPayments =
                await _db.Payments
                    .CountAsync(x =>
                        x.Booking != null &&
                        x.Booking.CustomerId ==
                            customerId);

            var unreadNotifications =
                await _db.Notifications
                    .CountAsync(x =>
                        x.CustomerId ==
                            customerId &&
                        !x.IsRead);

            return new CustomerDashboardDto
            {
                UpcomingBookings =
                    upcomingBookings,

                ReservedParking =
                    reservedParking,

                RecentPayments =
                    recentPayments,

                UnreadNotifications =
                    unreadNotifications
            };
        }

        public async Task<AdminDashboardDto>
            GetAdminAsync()
        {
            var totalRevenue =
                await _db.Payments
                    .Where(x =>
                        x.Status == "Completed")
                    .SumAsync(x =>
                        (decimal?)x.Amount)
                ?? 0;

            return new AdminDashboardDto
            {
                TotalEvents =
                    await _db.Events
                        .CountAsync(),

                TotalBookings =
                    await _db.Bookings
                        .CountAsync(),

                AvailableSeats =
                    await _db.Seats
                        .CountAsync(x =>
                            x.Status ==
                                "Available"),

                OccupiedParkingSlots =
                    await _db.ParkingSlots
                        .CountAsync(x =>
                            x.Status ==
                                "Occupied"),

                TotalRevenue =
                    totalRevenue,

                TotalCustomers =
                    await _db.Customers
                        .CountAsync(x =>
                            x.Role ==
                                "Customer")
            };
        }
    }
}