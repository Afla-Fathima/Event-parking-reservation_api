using EventParkingReservation.Data;
using EventParkingReservation.DTOs.Dashboard;
using EventParkingReservation.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservation.Repositories.Implementations
{
    public class DashboardRepository :
        IDashboardRepository
    {
        private readonly ApplicationDbContext _context;

        public DashboardRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CustomerDashboardDto>
            GetCustomerDashboardAsync(
                int customerId)
        {
            DateOnly today =
                DateOnly.FromDateTime(DateTime.Today);

            return new CustomerDashboardDto
            {
                UpcomingBookings =
                    await _context.Bookings
                        .Include(x => x.Event)
                        .CountAsync(x =>
                            x.CustomerId ==
                                customerId &&
                            x.Status !=
                                "Cancelled" &&
                            x.Event != null &&
                            x.Event.EventDate >=
                                today),

                ReservedParking =
                    await _context
                        .ParkingReservations
                        .Include(x => x.Booking)
                        .CountAsync(x =>
                            x.Booking != null &&
                            x.Booking.CustomerId ==
                                customerId &&
                            x.Status ==
                                "Active"),

                RecentPayments =
                    await _context.Payments
                        .Include(x => x.Booking)
                        .CountAsync(x =>
                            x.Booking != null &&
                            x.Booking.CustomerId ==
                                customerId),

                UnreadNotifications =
                    await _context
                        .Notifications
                        .CountAsync(x =>
                            x.CustomerId ==
                                customerId &&
                            !x.IsRead)
            };
        }

        public async Task<AdminDashboardDto>
            GetAdminDashboardAsync()
        {
            return new AdminDashboardDto
            {
                TotalEvents =
                    await _context.Events.CountAsync(),

                TotalBookings =
                    await _context.Bookings.CountAsync(),

                AvailableSeats =
                    await _context.Seats.CountAsync(x =>
                        x.Status == "Available"),

                OccupiedParkingSlots =
                    await _context.ParkingSlots
                        .CountAsync(x =>
                            x.Status ==
                                "Occupied"),

                TotalRevenue =
                    await _context.Payments
                        .Where(x =>
                            x.Status ==
                                "Completed")
                        .SumAsync(x =>
                            (decimal?)x.Amount)
                    ?? 0,

                TotalCustomers =
                    await _context.Customers
                        .CountAsync(x =>
                            x.Role ==
                                "Customer")
            };
        }
    }
}
