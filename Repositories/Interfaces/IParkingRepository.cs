using EventParkingReservation.Models;

namespace EventParkingReservation.Repositories.Interfaces
{
    public interface IParkingRepository
    {
        Task<IEnumerable<ParkingSlot>> GetByEventIdAsync(int eventId);

        Task<ParkingSlot?> GetByIdAsync(int parkingSlotId);

        Task<bool> SlotNumberExistsAsync(
            int eventId,
            string slotNumber,
            int? excludeId = null);

        Task<bool> HasActiveReservationAsync(int parkingSlotId);

        Task AddAsync(ParkingSlot slot);

        Task UpdateAsync(ParkingSlot slot);

        Task DeleteAsync(ParkingSlot slot);
    }
}