namespace EventParkingReservation.DTOs.Payment
{
    public class PaymentDto
    {
        public int PaymentId { get; set; }

        public int BookingId { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string TransactionReference { get; set; } = string.Empty;
    }

    public class PaymentStatusDto
    {
        public int BookingId { get; set; }

        public decimal AmountDue { get; set; }

        public string PaymentStatus { get; set; } = string.Empty;
    }
}
