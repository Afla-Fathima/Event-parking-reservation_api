using EventParkingReservation.Models;

namespace EventParkingReservation.Repositories.Interfaces
{
    public interface ISeatRepository
    {
        Task<IEnumerable<Seat>> GetByEventIdAsync(int eventId);

        Task<Seat?> GetByIdAsync(int seatId);

        Task<Event?> GetEventByIdAsync(int eventId);

        Task<bool> SeatNumberExistsAsync(
            int eventId,
            string seatNumber,
            int? excludeSeatId = null);

        Task<int> CountByEventAsync(int eventId);

        Task<bool> HasBookingsAsync(int seatId);

        Task AddAsync(Seat seat);

        Task AddRangeAsync(IEnumerable<Seat> seats);

        Task UpdateAsync(Seat seat);

        Task DeleteAsync(Seat seat);
    }
}