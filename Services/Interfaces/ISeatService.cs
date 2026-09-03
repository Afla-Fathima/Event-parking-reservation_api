using EventParkingReservation.DTOs.Seat;

namespace EventParkingReservation.Services.Interfaces
{
    public interface ISeatService
    {
        Task<IEnumerable<SeatResponseDto>>
            GetByEventIdAsync(int eventId);

        Task<SeatResponseDto?>
            GetByIdAsync(int seatId);

        Task<SeatResponseDto>
            AddAsync(CreateSeatDto dto);

        Task<SeatResponseDto>
            UpdateAsync(
                int seatId,
                UpdateSeatDto dto);

        Task DeleteAsync(int seatId);
    }
}