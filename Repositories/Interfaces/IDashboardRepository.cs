using EventParkingReservation.DTOs.Dashboard;

namespace EventParkingReservation.Repositories.Interfaces
{
    public interface IDashboardRepository
    {
        Task<CustomerDashboardDto> GetCustomerDashboardAsync(
            int customerId);

        Task<AdminDashboardDto> GetAdminDashboardAsync();
    }
}
