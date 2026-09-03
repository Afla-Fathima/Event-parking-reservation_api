using EventParkingReservation.Data;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservation.Repositories.Implementations
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Payment?> GetByIdAsync(
            int paymentId)
        {
            return await _context.Payments
                .Include(x => x.Booking)
                    .ThenInclude(x => x!.Customer)
                .Include(x => x.Booking)
                    .ThenInclude(x => x!.Event)
                .FirstOrDefaultAsync(x =>
                    x.PaymentId == paymentId);
        }

        public async Task<Payment?>
            GetByBookingIdAsync(int bookingId)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(x =>
                    x.BookingId == bookingId);
        }

        public async Task<IEnumerable<Payment>>
            GetByCustomerAsync(int customerId)
        {
            return await _context.Payments
                .Include(x => x.Booking)
                .Where(x =>
                    x.Booking != null &&
                    x.Booking.CustomerId ==
                        customerId)
                .OrderByDescending(x =>
                    x.PaymentDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Payment> CreateAsync(
            Payment payment,
            Booking booking)
        {
            await using var transaction =
                await _context.Database
                    .BeginTransactionAsync();

            try
            {
                bool exists =
                    await _context.Payments
                        .AnyAsync(x =>
                            x.BookingId ==
                                booking.BookingId);

                if (exists)
                {
                    throw new InvalidOperationException(
                        "Payment already exists for this booking.");
                }

                _context.Payments.Add(payment);

                booking.PaymentStatus =
                    "Completed";

                booking.Status =
                    "Confirmed";

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return payment;
            }
            catch
            {
                await transaction.RollbackAsync();

                throw;
            }
        }
    }
}
