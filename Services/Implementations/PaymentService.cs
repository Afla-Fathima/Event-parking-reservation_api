using EventParkingReservation.DTOs.Payment;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Interfaces;
using EventParkingReservation.Services.Interfaces;

namespace EventParkingReservation.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository
            _paymentRepository;

        private readonly IBookingRepository
            _bookingRepository;

        private readonly INotificationService
            _notificationService;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IBookingRepository bookingRepository,
            INotificationService notificationService)
        {
            _paymentRepository =
                paymentRepository;

            _bookingRepository =
                bookingRepository;

            _notificationService =
                notificationService;
        }

        public async Task<PaymentStatusDto>
            GetStatusAsync(
                int bookingId,
                int? customerId)
        {
            var booking =
                await _bookingRepository
                    .GetByIdAsync(bookingId)
                ?? throw new KeyNotFoundException(
                    "Booking not found.");

            EnsureAccess(
                booking,
                customerId);

            return new PaymentStatusDto
            {
                BookingId =
                    booking.BookingId,

                AmountDue =
                    booking.TotalAmount,

                PaymentStatus =
                    booking.PaymentStatus
            };
        }

        public async Task<PaymentDto> PayAsync(
            int bookingId,
            int? customerId,
            CreatePaymentDto dto)
        {
            var booking =
                await _bookingRepository
                    .GetByIdAsync(bookingId)
                ?? throw new KeyNotFoundException(
                    "Booking not found.");

            EnsureAccess(
                booking,
                customerId);

            if (booking.Status ==
                "Cancelled")
            {
                throw new InvalidOperationException(
                    "Cancelled booking cannot be paid.");
            }

            if (booking.Status ==
                "Expired")
            {
                throw new InvalidOperationException(
                    "Expired booking cannot be paid.");
            }

            if (booking.Status ==
                "Confirmed")
            {
                throw new InvalidOperationException(
                    "Booking is already confirmed.");
            }

            if (booking.HoldExpiresAt.HasValue &&
                booking.HoldExpiresAt.Value <=
                    DateTime.UtcNow)
            {
                throw new InvalidOperationException(
                    "Booking hold has expired.");
            }

            var existing =
                await _paymentRepository
                    .GetByBookingIdAsync(
                        bookingId);

            if (existing != null)
            {
                throw new InvalidOperationException(
                    "Payment already completed for this booking.");
            }

            if (booking.TotalAmount <= 0)
            {
                throw new InvalidOperationException(
                    "Booking amount is invalid.");
            }

            var payment =
                new Payment
                {
                    BookingId =
                        bookingId,

                    Amount =
                        booking.TotalAmount,

                    PaymentDate =
                        DateTime.UtcNow,

                    PaymentMethod =
                        string.IsNullOrWhiteSpace(
                            dto.PaymentMethod)
                            ? "Simulation"
                            : dto.PaymentMethod
                                .Trim(),

                    Status =
                        "Completed",

                    TransactionReference =
                        $"PAY-{Guid.NewGuid().ToString("N")[..12].ToUpperInvariant()}"
                };

            booking.HoldExpiresAt =
                null;

            await _paymentRepository
                .CreateAsync(
                    payment,
                    booking);

            await _notificationService
                .CreateAsync(
                    booking.CustomerId,
                    "Payment Completed",
                    $"Payment for booking {booking.BookingNumber} was completed successfully. Your booking is confirmed.",
                    "Payment");

            return Map(payment);
        }

        public async Task<IEnumerable<PaymentDto>>
            GetByCustomerAsync(
                int customerId)
        {
            var payments =
                await _paymentRepository
                    .GetByCustomerAsync(
                        customerId);

            return payments.Select(Map);
        }

        public async Task<ReceiptDto>
            GetReceiptAsync(
                int paymentId,
                int? customerId)
        {
            var payment =
                await _paymentRepository
                    .GetByIdAsync(paymentId)
                ?? throw new KeyNotFoundException(
                    "Payment not found.");

            if (customerId.HasValue &&
                payment.Booking?.CustomerId !=
                    customerId.Value)
            {
                throw new UnauthorizedAccessException(
                    "You cannot access this receipt.");
            }

            return new ReceiptDto
            {
                BookingNumber =
                    payment.Booking
                        ?.BookingNumber
                    ?? string.Empty,

                CustomerName =
                    payment.Booking
                        ?.Customer?.Name
                    ?? string.Empty,

                EventName =
                    payment.Booking
                        ?.Event?.EventName
                    ?? string.Empty,

                Amount =
                    payment.Amount,

                PaymentDate =
                    payment.PaymentDate,

                PaymentMethod =
                    payment.PaymentMethod,

                TransactionReference =
                    payment.TransactionReference
            };
        }

        private static void EnsureAccess(
            Booking booking,
            int? customerId)
        {
            // null = Administrator
            if (customerId.HasValue &&
                booking.CustomerId !=
                    customerId.Value)
            {
                throw new UnauthorizedAccessException(
                    "You cannot access this booking payment.");
            }
        }

        private static PaymentDto Map(
            Payment payment)
        {
            return new PaymentDto
            {
                PaymentId =
                    payment.PaymentId,

                BookingId =
                    payment.BookingId,

                Amount =
                    payment.Amount,

                PaymentDate =
                    payment.PaymentDate,

                PaymentMethod =
                    payment.PaymentMethod,

                Status =
                    payment.Status,

                TransactionReference =
                    payment.TransactionReference
            };
        }
    }
}