using EventParkingReservation.Models;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservation.Data
{
    public static class AdminSeeder
    {
        public static async Task SeedAsync(
            ApplicationDbContext context)
        {
            const string adminEmail = "admin@gmail.com";
            const string adminPassword = "Admin@123";

            var existingUser = await context.Customers
                .FirstOrDefaultAsync(x => x.Email == adminEmail);

            // Existing account irundha Admin-aa update pannum
            if (existingUser != null)
            {
                existingUser.Role = "Admin";
                existingUser.Status = "Active";
                existingUser.EmailVerified = true;

                // Development admin password
                existingUser.PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(adminPassword);

                await context.SaveChangesAsync();

                return;
            }

            // Account illana pudhusa Admin create pannum
            var admin = new Customer
            {
                Name = "System Admin",
                Email = adminEmail,
                PhoneNumber = "0771234567",

                PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(adminPassword),

                Role = "Admin",
                Status = "Active",
                EmailVerified = true,

                EmailVerificationToken = null,
                EmailVerificationTokenExpiresAt = null,

                PasswordResetToken = null,
                PasswordResetTokenExpiresAt = null,

                CreatedDate = DateTime.UtcNow
            };

            context.Customers.Add(admin);

            await context.SaveChangesAsync();
        }
    }
}