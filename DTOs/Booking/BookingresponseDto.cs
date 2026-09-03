namespace EventParkingReservation.DTOs.Booking
{
    public class BookingResponseDto
    {
        public int BookingId { get; set; }

        public string BookingNumber { get; set; } = string.Empty;

        public int CustomerId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public int EventId { get; set; }

        public string EventName { get; set; } = string.Empty;

        public DateTime BookingDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public string PaymentStatus { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public List<string> Seats { get; set; }
            = new List<string>();

        public string? ParkingSlot { get; set; }
    }
}
