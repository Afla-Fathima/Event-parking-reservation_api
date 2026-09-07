using EventParkingReservation.Models;

namespace EventParkingReservation.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        Task<Booking> CreateReservationAsync(
            Booking booking,
            IReadOnlyCollection<int> seatIds,
            int? parkingSlotId);

        Task<Booking> AddSeatsAsync(
            int bookingId,
            IReadOnlyCollection<int> seatIds);

        Task<Booking> ReserveParkingAsync(
            int bookingId,
            int parkingSlotId);

        Task<Booking> RemoveParkingAsync(
            int bookingId);

        Task<Booking?> GetByIdAsync(
            int id);

        Task<List<Booking>> GetByCustomerAsync(
            int customerId);

        Task<List<Booking>> GetByEventAsync(
            int eventId);

        Task CancelAsync(
            Booking booking);

        Task<bool> CustomerExistsAsync(
            int customerId);

        Task<Event?> GetEventAsync(
            int eventId);
    }
}