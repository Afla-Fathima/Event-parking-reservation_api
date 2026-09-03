using EventParkingReservation.DTOs.Dashboard;

namespace EventParkingReservation.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<CustomerDashboardDto>
            GetCustomerDashboardAsync(
                int customerId);

        Task<AdminDashboardDto>
            GetAdminDashboardAsync();
    }
}
