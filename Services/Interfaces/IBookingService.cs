using EventParkingReservation.DTOs.Booking;

namespace EventParkingReservation.Services.Interfaces
{
    public interface IBookingService
    {
        Task<BookingResponseDto> CreateAsync(
            CreateBookingDto dto);

        Task<BookingResponseDto> AddSeatsAsync(
            int bookingId,
            int customerId,
            AttachSeatsDto dto);

        Task<BookingResponseDto> ReserveParkingAsync(
            int bookingId,
            int customerId,
            ReserveParkingDto dto);

        Task<BookingResponseDto> RemoveParkingAsync(
            int bookingId,
            int customerId);

        Task<BookingResponseDto?> GetByIdAsync(
            int id);

        Task<List<BookingResponseDto>>
            GetByCustomerAsync(
                int customerId);

        Task<List<BookingResponseDto>>
            GetByEventAsync(
                int eventId);

        Task CancelAsync(
            int id);
    }
}