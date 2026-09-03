using EventParkingReservation.DTOs.ParkingSlot;

namespace EventParkingReservation.Services.Interfaces
{
    public interface IParkingSlotService
    {
        Task<IEnumerable<ParkingSlotResponseDto>>
            GetAllAsync();

        Task<IEnumerable<ParkingSlotResponseDto>>
            GetByEventIdAsync(int eventId);

        Task<ParkingSlotResponseDto?>
            GetByIdAsync(int id);

        Task<ParkingSlotResponseDto>
            AddAsync(CreateParkingSlotDto dto);

        Task<ParkingSlotResponseDto>
            UpdateAsync(
                int id,
                UpdateParkingSlotDto dto);

        Task DeleteAsync(int id);
    }
}