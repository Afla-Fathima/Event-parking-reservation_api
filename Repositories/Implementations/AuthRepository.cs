using EventParkingReservation.Data;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservation.Repositories.Implementations
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Customer?> GetByEmailAsync(
            string email)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c =>
                    c.Email == email);
        }

        public async Task<Customer?> GetByIdAsync(
            int customerId)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c =>
                    c.CustomerId == customerId);
        }

        public async Task<Customer?> GetByPasswordResetTokenAsync(
            string token)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c =>
                    c.PasswordResetToken == token);
        }

        public async Task<bool> EmailExistsAsync(
            string email)
        {
            return await _context.Customers
                .AnyAsync(c =>
                    c.Email == email);
        }

        public async Task<Customer> AddAsync(
            Customer customer)
        {
            _context.Customers.Add(customer);

            await _context.SaveChangesAsync();

            return customer;
        }

        public async Task UpdateAsync(
            Customer customer)
        {
            _context.Customers.Update(customer);

            await _context.SaveChangesAsync();
        }
    }
}