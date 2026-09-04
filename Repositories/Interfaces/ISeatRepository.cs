using EventParkingReservation.Models;

namespace EventParkingReservation.Repositories.Interfaces
{
    public interface ISeatRepository
    {
        Task<List<Seat>> GetByEventIdAsync(
            int eventId);

        Task<Seat?> GetByIdAsync(
            int seatId);

        Task<Event?> GetEventAsync(
            int eventId);

        Task<int> CountByEventAsync(
            int eventId);

        Task<bool> NumberExistsAsync(
            int eventId,
            string number,
            int? excludeId = null);

        Task<bool> HasActiveBookingAsync(
            int seatId);

        Task AddAsync(
            Seat seat);

        Task UpdateAsync(
            Seat seat);

        Task DeleteAsync(
            Seat seat);
    }
}