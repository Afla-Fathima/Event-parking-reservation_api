using EventParkingReservation.DTOs.Payment;

namespace EventParkingReservation.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentStatusDto> GetStatusAsync(int bookingId);

        Task<PaymentDto> PayAsync(
            int bookingId,
            CreatePaymentDto dto);

        Task<IEnumerable<PaymentDto>> GetByCustomerAsync(
            int customerId);

        Task<ReceiptDto> GetReceiptAsync(int paymentId);
    }
}