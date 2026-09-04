using EventParkingReservation.DTOs.Dashboard;

namespace EventParkingReservation.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<CustomerDashboardDto> GetCustomerAsync(
            int customerId);

        Task<AdminDashboardDto> GetAdminAsync();
    }
}