using EventParkingReservation.DTOs.Booking;

namespace EventParkingReservation.Services.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingResponseDto>>
            GetAllAsync(int? eventId);

        Task<IEnumerable<BookingResponseDto>>
            GetByCustomerAsync(int customerId);

        Task<BookingResponseDto> GetByIdAsync(
            int bookingId);

        Task<BookingResponseDto> CreateAsync(
            CreateBookingDto dto);

        Task AddSeatsAsync(
            int bookingId,
            AttachSeatsDto dto);

        Task ReserveParkingAsync(
            int bookingId,
            ReserveParkingDto dto);

        Task RemoveParkingAsync(int bookingId);

        Task CancelAsync(int bookingId);
    }
}
