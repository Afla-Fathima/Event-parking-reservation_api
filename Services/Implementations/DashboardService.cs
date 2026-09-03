using EventParkingReservation.DTOs.Dashboard;
using EventParkingReservation.Repositories.Interfaces;
using EventParkingReservation.Services.Interfaces;

namespace EventParkingReservation.Services.Implementations
{
    public class DashboardService :
        IDashboardService
    {
        private readonly IDashboardRepository
            _repository;

        public DashboardService(
            IDashboardRepository repository)
        {
            _repository = repository;
        }

        public Task<CustomerDashboardDto>
            GetCustomerDashboardAsync(
                int customerId)
        {
            return _repository
                .GetCustomerDashboardAsync(
                    customerId);
        }

        public Task<AdminDashboardDto>
            GetAdminDashboardAsync()
        {
            return _repository
                .GetAdminDashboardAsync();
        }
    }
}
