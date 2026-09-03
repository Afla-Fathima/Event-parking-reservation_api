using System.Text.Json.Serialization;

namespace EventParkingReservation.Models
{
    public class Seat
    {
        public int SeatId { get; set; }

        public int EventId { get; set; }

        public string SeatNumber { get; set; }
            = string.Empty;

        public string SeatType { get; set; }
            = string.Empty;

        public decimal Price { get; set; }

        public string Status { get; set; }
            = "Available";


        // Navigation Property
        [JsonIgnore]
        public Event Event { get; set; }
            = null!;


        public ICollection<BookingSeat> BookingSeats
        {
            get;
            set;
        } = new List<BookingSeat>();
    }
}