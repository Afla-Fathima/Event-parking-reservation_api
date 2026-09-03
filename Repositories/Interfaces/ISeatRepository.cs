using EventParkingReservation.Models;

namespace EventParkingReservation.Repositories.Interfaces
{
    public interface ISeatRepository
    {
        Task<IEnumerable<Seat>> GetByEventIdAsync(int eventId);

        Task<Seat?> GetByIdAsync(int seatId);

        Task<bool> SeatNumberExistsAsync(
            int eventId,
            string seatNumber,
            int? excludeSeatId = null);

        Task<bool> HasActiveBookingAsync(int seatId);

        Task AddAsync(Seat seat);

        Task UpdateAsync(Seat seat);

        Task DeleteAsync(Seat seat);
    }
}
