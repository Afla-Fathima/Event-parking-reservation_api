using EventParkingReservation.Models;

namespace EventParkingReservation.Repositories.Interfaces
{
    public interface IParkingSlotRepository
    {
        Task<IEnumerable<ParkingSlot>>
            GetAllAsync();

        Task<IEnumerable<ParkingSlot>>
            GetByEventIdAsync(int eventId);

        Task<ParkingSlot?>
            GetByIdAsync(int id);

        Task<Event?>
            GetEventByIdAsync(int eventId);

        Task<bool>
            EventExistsAsync(int eventId);

        Task<bool>
            SlotNumberExistsAsync(
                int eventId,
                string slotNumber,
                int? excludeId = null);

        Task<bool>
            HasReservationAsync(
                int parkingSlotId);

        Task<int>
            CountByEventAsync(int eventId);

        Task AddAsync(
            ParkingSlot parkingSlot);

        Task AddRangeAsync(
            IEnumerable<ParkingSlot> parkingSlots);

        Task UpdateAsync(
            ParkingSlot parkingSlot);

        Task DeleteAsync(
            ParkingSlot parkingSlot);
    }
}