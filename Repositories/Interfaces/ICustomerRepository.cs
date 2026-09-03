using EventParkingReservation.Models;

namespace EventParkingReservation.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllAsync(string? search = null);
        Task<Customer?> GetByIdAsync(int id);
        Task<Customer?> GetByEmailAsync(string email);
        Task UpdateAsync(Customer customer);
        Task DeactivateAsync(Customer customer);
    }
}
