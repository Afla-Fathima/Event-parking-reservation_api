using EventParkingReservation.Services.Implementations;
using EventParkingReservation.DTOs.Payment;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Implementations;
using EventParkingReservation.Repositories.Interfaces;
using EventParkingReservation.Services.Interfaces;

namespace EventParkingReservation.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly INotificationService _notificationService;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IBookingRepository bookingRepository,
            INotificationService notificationService)
        {
            _paymentRepository = paymentRepository;
            _bookingRepository = bookingRepository;
            _notificationService = notificationService;
        }

        public async Task<PaymentStatusDto> GetStatusAsync(
            int bookingId)
        {
            var booking =
                await _bookingRepository.GetByIdAsync(bookingId)
                ?? throw new KeyNotFoundException(
                    "Booking not found.");

            return new PaymentStatusDto
            {
                BookingId = booking.BookingId,
                AmountDue = booking.TotalAmount,
                PaymentStatus = booking.PaymentStatus
            };
        }

        public async Task<PaymentDto> PayAsync(
            int bookingId,
            CreatePaymentDto dto)
        {
            var booking =
                await _bookingRepository.GetByIdAsync(bookingId)
                ?? throw new KeyNotFoundException(
                    "Booking not found.");

            if (booking.Status == "Cancelled")
            {
                throw new InvalidOperationException(
                    "Cancelled booking cannot be paid.");
            }

            var existing =
                await _paymentRepository
                    .GetByBookingIdAsync(bookingId);

            if (existing != null)
            {
                throw new InvalidOperationException(
                    "Payment already completed for this booking.");
            }

            var payment = new Payment
            {
                BookingId = bookingId,
                Amount = booking.TotalAmount,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = dto.PaymentMethod,
                Status = "Completed",

                TransactionReference =
                    $"PAY-{Guid.NewGuid()
                        .ToString("N")[..12]
                        .ToUpper()}"
            };

            await _paymentRepository.CreateAsync(
                payment,
                booking);

            await _notificationService.CreateAsync(
                booking.CustomerId,
                "Payment Completed",
                $"Payment for booking {booking.BookingNumber} was completed successfully.",
                "Payment");

            return Map(payment);
        }

        public async Task<IEnumerable<PaymentDto>>
            GetByCustomerAsync(int customerId)
        {
            var payments =
                await _paymentRepository
                    .GetByCustomerAsync(customerId);

            return payments.Select(Map);
        }

        public async Task<ReceiptDto> GetReceiptAsync(
            int paymentId)
        {
            var payment =
                await _paymentRepository.GetByIdAsync(paymentId)
                ?? throw new KeyNotFoundException(
                    "Payment not found.");

            return new ReceiptDto
            {
                BookingNumber =
                    payment.Booking?.BookingNumber
                    ?? string.Empty,

                CustomerName =
                    payment.Booking?.Customer?.Name
                    ?? string.Empty,

                EventName =
                    payment.Booking?.Event?.EventName
                    ?? string.Empty,

                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                PaymentMethod = payment.PaymentMethod,

                TransactionReference =
                    payment.TransactionReference
            };
        }

        private static PaymentDto Map(
            Payment payment)
        {
            return new PaymentDto
            {
                PaymentId = payment.PaymentId,
                BookingId = payment.BookingId,
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status,

                TransactionReference =
                    payment.TransactionReference
            };
        }
    }
}
