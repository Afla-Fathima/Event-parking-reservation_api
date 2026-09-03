using EventParkingReservation.DTOs.Seat;

namespace EventParkingReservation.Services.Interfaces
{
    public interface ISeatService
    {
        Task<IEnumerable<SeatDto>>
            GetByEventAsync(int eventId);

        Task<SeatDto> CreateAsync(
            int eventId,
            CreateSeatDto dto);

        Task<SeatDto> UpdateAsync(
            int eventId,
            int seatId,
            UpdateSeatDto dto);

        Task DeleteAsync(
            int eventId,
            int seatId);
    }
}
