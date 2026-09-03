using EventParkingReservation.Data;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservation.Repositories.Implementations
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync(
            string? search = null)
        {
            IQueryable<Customer> query =
                _context.Customers.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.Name.Contains(search) ||
                    x.Email.Contains(search));
            }

            return await query
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(x => x.CustomerId == id);
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);

            await _context.SaveChangesAsync();
        }

        public async Task DeactivateAsync(Customer customer)
        {
            customer.Status = "Inactive";

            _context.Customers.Update(customer);

            await _context.SaveChangesAsync();
        }
    }
}
