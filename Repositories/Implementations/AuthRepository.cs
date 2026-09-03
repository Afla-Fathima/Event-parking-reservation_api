using EventParkingReservation.Data;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservation.Repositories.Implementations
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<Customer?> GetByIdAsync(int customerId)
        {
            return await _context.Customers.FindAsync(customerId);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Customers
                .AnyAsync(x => x.Email == email);
        }

        public async Task<Customer> AddAsync(Customer customer)
        {
            _context.Customers.Add(customer);

            await _context.SaveChangesAsync();

            return customer;
        }
    }
}
