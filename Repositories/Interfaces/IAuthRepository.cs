using EventParkingReservation.Models;

namespace EventParkingReservation.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<Customer?> GetByEmailAsync(string email);
        Task<Customer?> GetByIdAsync(int customerId);
        Task<bool> EmailExistsAsync(string email);
        Task<Customer> AddAsync(Customer customer);
    }
}
