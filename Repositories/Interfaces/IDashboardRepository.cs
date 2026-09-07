using EventParkingReservation.DTOs.Dashboard;

namespace EventParkingReservation.Repositories.Interfaces
{
    public interface IDashboardRepository
    {
        Task<CustomerDashboardDto> GetCustomerAsync(
            int customerId);

        Task<AdminDashboardDto> GetAdminAsync();
    }
}