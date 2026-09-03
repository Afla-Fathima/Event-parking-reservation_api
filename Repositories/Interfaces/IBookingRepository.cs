using EventParkingReservation.Models;

namespace EventParkingReservation.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetAllAsync(
            int? eventId = null);

        Task<IEnumerable<Booking>> GetByCustomerAsync(
            int customerId);

        Task<Booking?> GetByIdAsync(int bookingId);

        Task<Booking> CreateReservationAsync(
            Booking booking,
            IEnumerable<int> seatIds,
            int? parkingSlotId);

        Task AddSeatsAsync(
            int bookingId,
            IEnumerable<int> seatIds);

        Task ReserveParkingAsync(
            int bookingId,
            int parkingSlotId);

        Task RemoveParkingAsync(int bookingId);

        Task CancelAsync(Booking booking);

        Task UpdateAsync(Booking booking);
    }
}