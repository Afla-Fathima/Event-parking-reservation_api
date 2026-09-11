using EventParkingReservation.DTOs.Parking;

namespace EventParkingReservation.Services.Interfaces
{
    public interface IParkingService
    {
        Task<List<ParkingSlotDto>>
            GetByEventIdAsync(
                int eventId);

        Task<ParkingSlotDto>
            CreateAsync(
                int eventId,
                CreateParkingSlotDto dto);

        Task<ParkingSlotDto>
            UpdateAsync(
                int eventId,
                int slotId,
                UpdateParkingSlotDto dto);

        Task DeleteAsync(
            int eventId,
            int slotId);

        // NEW
        // Makes total parking count = 20
        Task<int>
            GenerateDefaultSlotsAsync(
                int eventId);
    }
}