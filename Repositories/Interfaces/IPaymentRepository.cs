using EventParkingReservation.Models;

namespace EventParkingReservation.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(int paymentId);

        Task<Payment?> GetByBookingIdAsync(int bookingId);

        Task<IEnumerable<Payment>> GetByCustomerAsync(int customerId);

        Task<Payment> CreateAsync(
            Payment payment,
            Booking booking);
    }
}
