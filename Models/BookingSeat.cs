using System.Text.Json.Serialization;

namespace EventParkingReservation.Models
{
    public class BookingSeat
    {
        public int BookingSeatId { get; set; }

        public int BookingId { get; set; }

        public int SeatId { get; set; }

        public decimal SeatPrice { get; set; }

        public string Status { get; set; }
            = "Reserved";

        public DateTime CreatedDate { get; set; }
            = DateTime.Now;


        // Prevent BookingSeat -> Booking -> BookingSeats
        [JsonIgnore]
        public Booking Booking { get; set; }
            = null!;


        public Seat Seat { get; set; }
            = null!;
    }
}