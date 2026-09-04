using EventParkingReservation.DTOs.Payment;

namespace EventParkingReservation.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentStatusDto> GetStatusAsync(
            int bookingId,
            int? customerId);

        Task<PaymentDto> PayAsync(
            int bookingId,
            int? customerId,
            CreatePaymentDto dto);

        Task<IEnumerable<PaymentDto>>
            GetByCustomerAsync(
                int customerId);

        Task<ReceiptDto> GetReceiptAsync(
            int paymentId,
            int? customerId);
    }
}