namespace EventParkingReservation.DTOs.Seat
{
    public class SeatDto
    {
        public int SeatId { get; set; }

        public int EventId { get; set; }

        public string SeatNumber { get; set; } = string.Empty;

        public string SeatType { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
