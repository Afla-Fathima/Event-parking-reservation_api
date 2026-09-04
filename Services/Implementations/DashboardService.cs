using EventParkingReservation.DTOs.Dashboard;
using EventParkingReservation.Repositories.Interfaces;
using EventParkingReservation.Services.Interfaces;

namespace EventParkingReservation.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _repository;

        public DashboardService(
            IDashboardRepository repository)
        {
            _repository = repository;
        }

        public async Task<CustomerDashboardDto>
            GetCustomerAsync(
                int customerId)
        {
            return await _repository
                .GetCustomerAsync(customerId);
        }

        public async Task<AdminDashboardDto>
            GetAdminAsync()
        {
            return await _repository
                .GetAdminAsync();
        }
    }
}