namespace EventParkingReservation.DTOs.Payment
{
    public class ReceiptDto
    {
        public string BookingNumber { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public string EventName { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public string TransactionReference { get; set; } = string.Empty;
    }
}
