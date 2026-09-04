using EventParkingReservation.DTOs.Booking;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Interfaces;
using EventParkingReservation.Services.Interfaces;

namespace EventParkingReservation.Services.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _repo;
        private readonly INotificationService
            _notifications;

        public BookingService(
            IBookingRepository repo,
            INotificationService notifications)
        {
            _repo = repo;
            _notifications = notifications;
        }

        public async Task<BookingResponseDto>
            CreateAsync(
                CreateBookingDto dto)
        {
            if (dto.SeatIds == null ||
                dto.SeatIds.Count == 0)
            {
                throw new InvalidOperationException(
                    "A booking must contain at least one seat.");
            }

            if (dto.SeatIds.Distinct().Count() !=
                dto.SeatIds.Count)
            {
                throw new InvalidOperationException(
                    "The same seat cannot be selected twice.");
            }

            if (!await _repo.CustomerExistsAsync(
                    dto.CustomerId))
            {
                throw new InvalidOperationException(
                    "Customer not found or inactive.");
            }

            var eventItem =
                await _repo.GetEventAsync(
                    dto.EventId);

            if (eventItem == null)
            {
                throw new KeyNotFoundException(
                    "Event not found.");
            }

            var booking =
                new Booking
                {
                    BookingNumber =
                        $"BKG-{DateTime.UtcNow:yyyyMMddHHmmss}-" +
                        Guid.NewGuid()
                            .ToString("N")[..5]
                            .ToUpperInvariant(),

                    CustomerId =
                        dto.CustomerId,

                    EventId =
                        dto.EventId,

                    BookingDate =
                        DateTime.UtcNow,

                    HoldExpiresAt =
                        DateTime.UtcNow
                            .AddMinutes(15),

                    Status =
                        "Pending",

                    PaymentStatus =
                        "Pending",

                    TotalAmount =
                        0
                };

            var created =
                await _repo
                    .CreateReservationAsync(
                        booking,
                        dto.SeatIds,
                        dto.ParkingSlotId);

            await _notifications.CreateAsync(
                created.CustomerId,
                "Booking Created",
                $"Booking {created.BookingNumber} is pending payment.",
                "Booking");

            return Map(created);
        }

        public async Task<BookingResponseDto>
            AddSeatsAsync(
                int bookingId,
                int customerId,
                AttachSeatsDto dto)
        {
            var booking =
                await RequireOwnedBookingAsync(
                    bookingId,
                    customerId);

            EnsurePending(booking);

            if (dto.SeatIds == null ||
                dto.SeatIds.Count == 0)
            {
                throw new InvalidOperationException(
                    "At least one seat is required.");
            }

            var updated =
                await _repo.AddSeatsAsync(
                    bookingId,
                    dto.SeatIds);

            return Map(updated);
        }

        public async Task<BookingResponseDto>
            ReserveParkingAsync(
                int bookingId,
                int customerId,
                ReserveParkingDto dto)
        {
            var booking =
                await RequireOwnedBookingAsync(
                    bookingId,
                    customerId);

            EnsurePending(booking);

            var updated =
                await _repo
                    .ReserveParkingAsync(
                        bookingId,
                        dto.ParkingSlotId);

            return Map(updated);
        }

        public async Task<BookingResponseDto>
            RemoveParkingAsync(
                int bookingId,
                int customerId)
        {
            var booking =
                await RequireOwnedBookingAsync(
                    bookingId,
                    customerId);

            EnsurePending(booking);

            var updated =
                await _repo
                    .RemoveParkingAsync(
                        bookingId);

            return Map(updated);
        }

        public async Task<BookingResponseDto?>
            GetByIdAsync(
                int id)
        {
            var booking =
                await _repo.GetByIdAsync(id);

            return booking == null
                ? null
                : Map(booking);
        }

        public async Task<List<BookingResponseDto>>
            GetByCustomerAsync(
                int customerId)
        {
            var bookings =
                await _repo.GetByCustomerAsync(
                    customerId);

            return bookings
                .Select(Map)
                .ToList();
        }

        public async Task<List<BookingResponseDto>>
            GetByEventAsync(
                int eventId)
        {
            var bookings =
                await _repo.GetByEventAsync(
                    eventId);

            return bookings
                .Select(Map)
                .ToList();
        }

        public async Task CancelAsync(
            int id)
        {
            var booking =
                await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(
                    "Booking not found.");

            if (booking.Status ==
                "Cancelled")
            {
                throw new InvalidOperationException(
                    "Booking is already cancelled.");
            }

            if (booking.Status ==
                "Expired")
            {
                throw new InvalidOperationException(
                    "Expired booking cannot be cancelled.");
            }

            await _repo.CancelAsync(
                booking);

            await _notifications.CreateAsync(
                booking.CustomerId,
                "Booking Cancelled",
                $"Booking {booking.BookingNumber} was cancelled.",
                "Cancellation");
        }

        private async Task<Booking>
            RequireOwnedBookingAsync(
                int bookingId,
                int customerId)
        {
            var booking =
                await _repo.GetByIdAsync(
                    bookingId)
                ?? throw new KeyNotFoundException(
                    "Booking not found.");

            if (booking.CustomerId !=
                customerId)
            {
                throw new UnauthorizedAccessException(
                    "You cannot access this booking.");
            }

            return booking;
        }

        private static void EnsurePending(
            Booking booking)
        {
            if (booking.Status != "Pending")
            {
                throw new InvalidOperationException(
                    "Only pending bookings can be modified.");
            }

            if (booking.HoldExpiresAt.HasValue &&
                booking.HoldExpiresAt.Value <=
                    DateTime.UtcNow)
            {
                throw new InvalidOperationException(
                    "Booking hold has expired.");
            }
        }

        private static BookingResponseDto Map(
            Booking booking)
        {
            return new BookingResponseDto
            {
                BookingId =
                    booking.BookingId,

                BookingNumber =
                    booking.BookingNumber,

                CustomerId =
                    booking.CustomerId,

                CustomerName =
                    booking.Customer?.Name
                    ?? string.Empty,

                EventId =
                    booking.EventId,

                EventName =
                    booking.Event?.EventName
                    ?? string.Empty,

                BookingDate =
                    booking.BookingDate,

                Status =
                    booking.Status,

                PaymentStatus =
                    booking.PaymentStatus,

                TotalAmount =
                    booking.TotalAmount,

                Seats =
                    booking.BookingSeats
                        .Where(x =>
                            x.Status ==
                                "Active")
                        .Select(x =>
                            x.Seat?.SeatNumber
                            ?? string.Empty)
                        .Where(x =>
                            !string.IsNullOrWhiteSpace(
                                x))
                        .ToList(),

                ParkingSlot =
                    booking.ParkingReservation != null &&
                    booking.ParkingReservation.Status ==
                        "Active"
                        ? booking
                            .ParkingReservation
                            .ParkingSlot
                            ?.SlotNumber
                        : null
            };
        }
    }
}