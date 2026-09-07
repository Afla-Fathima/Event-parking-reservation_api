using EventParkingReservation.Models;

namespace EventParkingReservation.Repositories.Interfaces
{
    public interface IParkingRepository
    {
        Task<List<ParkingSlot>> GetByEventIdAsync(
            int eventId);

        Task<ParkingSlot?> GetByIdAsync(
            int id);

        Task<Event?> GetEventAsync(
            int eventId);

        Task<bool> NumberExistsAsync(
            int eventId,
            string number,
            int? excludeId = null);

        Task<bool> HasActiveReservationAsync(
            int slotId);

        Task AddAsync(
            ParkingSlot slot);

        Task UpdateAsync(
            ParkingSlot slot);

        Task DeleteAsync(
            ParkingSlot slot);
    }
}