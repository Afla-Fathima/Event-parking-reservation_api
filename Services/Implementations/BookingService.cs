using EventParkingReservation.Services.Implementations;
using EventParkingReservation.DTOs.Booking;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Implementations;
using EventParkingReservation.Repositories.Interfaces;
using EventParkingReservation.Services.Interfaces;

namespace EventParkingReservation.Services.Implementations
{
    public class BookingService :
        IBookingService
    {
        private readonly IBookingRepository
        _bookingRepository;

        private readonly ICustomerRepository
        _customerRepository;

        private readonly IEventRepository
            _eventRepository;
        private readonly INotificationService
            _notificationService;

        public BookingService(
            IBookingRepository bookingRepository,
            ICustomerRepository customerRepository,
            IEventRepository eventRepository,
            INotificationService notificationService)
        {
            _bookingRepository =
                bookingRepository;

            _customerRepository =
                customerRepository;

            _eventRepository =
                eventRepository;

            _notificationService =
                notificationService;
        }

        public async Task<IEnumerable<BookingResponseDto>>
            GetAllAsync(int? eventId)
        {
            var bookings =
                await _bookingRepository
                    .GetAllAsync(eventId);

            return bookings.Select(Map);
        }

        public async Task<IEnumerable<BookingResponseDto>>
            GetByCustomerAsync(int customerId)
        {
            var bookings =
                await _bookingRepository
                    .GetByCustomerAsync(customerId);

            return bookings.Select(Map);
        }

        public async Task<BookingResponseDto>
            GetByIdAsync(int bookingId)
        {
            var booking =
                await _bookingRepository
                    .GetByIdAsync(bookingId)
                ?? throw new KeyNotFoundException(
                    "Booking not found.");

            return Map(booking);
        }

        public async Task<BookingResponseDto>
            CreateAsync(CreateBookingDto dto)
        {
            if (dto.SeatIds == null ||
                dto.SeatIds.Count == 0)
            {
                throw new InvalidOperationException(
                    "At least one seat must be selected.");
            }

            var customer =
                await _customerRepository
                    .GetByIdAsync(dto.CustomerId)
                ?? throw new KeyNotFoundException(
                    "Customer not found.");

            var eventEntity =
                await _eventRepository
                    .GetByIdAsync(dto.EventId)
                ?? throw new KeyNotFoundException(
                    "Event not found.");

            var booking = new Booking
            {
                BookingNumber =
                    GenerateBookingNumber(),

                CustomerId =
                    dto.CustomerId,

                EventId =
                    dto.EventId,

                BookingDate =
                    DateTime.UtcNow,

                Status =
                    "Pending",

                PaymentStatus =
                    "Pending"
            };

            await _bookingRepository
                .CreateReservationAsync(
                    booking,
                    dto.SeatIds,
                    dto.ParkingSlotId);

            await _notificationService
                .CreateAsync(
                    customer.CustomerId,
                    "Booking Created",
                    $"Booking {booking.BookingNumber} was created for {eventEntity.EventName}.",
                    "Booking");

            var result =
                await _bookingRepository
                    .GetByIdAsync(
                        booking.BookingId);

            return Map(result!);
        }

        public async Task AddSeatsAsync(
            int bookingId,
            AttachSeatsDto dto)
        {
            if (dto.SeatIds == null ||
                dto.SeatIds.Count == 0)
            {
                throw new InvalidOperationException(
                    "At least one seat is required.");
            }

            var booking =
                await _bookingRepository
                    .GetByIdAsync(bookingId)
                ?? throw new KeyNotFoundException(
                    "Booking not found.");

            if (booking.Status != "Pending")
            {
                throw new InvalidOperationException(
                    "Seats can only be changed while booking is pending.");
            }

            await _bookingRepository
                .AddSeatsAsync(
                    bookingId,
                    dto.SeatIds);
        }

        public async Task ReserveParkingAsync(
            int bookingId,
            ReserveParkingDto dto)
        {
            var booking =
                await _bookingRepository
                    .GetByIdAsync(bookingId)
                ?? throw new KeyNotFoundException(
                    "Booking not found.");

            if (booking.Status != "Pending")
            {
                throw new InvalidOperationException(
                    "Parking can only be changed while booking is pending.");
            }

            await _bookingRepository
                .ReserveParkingAsync(
                    bookingId,
                    dto.ParkingSlotId);
        }

        public async Task RemoveParkingAsync(
            int bookingId)
        {
            var booking =
                await _bookingRepository
                    .GetByIdAsync(bookingId)
                ?? throw new KeyNotFoundException(
                    "Booking not found.");

            if (booking.Status != "Pending")
            {
                throw new InvalidOperationException(
                    "Parking cannot be removed after booking is finalized.");
            }

            await _bookingRepository
                .RemoveParkingAsync(bookingId);
        }

        public async Task CancelAsync(
            int bookingId)
        {
            var booking =
                await _bookingRepository
                    .GetByIdAsync(bookingId)
                ?? throw new KeyNotFoundException(
                    "Booking not found.");

            if (booking.Status == "Cancelled")
            {
                throw new InvalidOperationException(
                    "Booking is already cancelled.");
            }

            await _bookingRepository
                .CancelAsync(booking);

            await _notificationService
                .CreateAsync(
                    booking.CustomerId,
                    "Booking Cancelled",
                    $"Booking {booking.BookingNumber} has been cancelled.",
                    "Cancellation");
        }

        private static string
            GenerateBookingNumber()
        {
            return
                $"BKG-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..5].ToUpper()}";
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
                    booking
                        .ParkingReservation?
                        .Status ==
                        "Active"
                        ? booking
                            .ParkingReservation
                            .ParkingSlot?
                            .SlotNumber
                        : null
            };
        }
    }
}
