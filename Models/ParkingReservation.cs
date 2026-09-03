using System.Text.Json.Serialization;

namespace EventParkingReservation.Models
{
    public class ParkingReservation
    {
        public int ParkingReservationId { get; set; }

        public int BookingId { get; set; }

        public int ParkingSlotId { get; set; }

        public string VehicleNumber { get; set; }
            = string.Empty;

        public DateTime ReservedDate { get; set; }
            = DateTime.Now;

        public string Status { get; set; }
            = "Reserved";


        // Prevent cycle
        [JsonIgnore]
        public Booking Booking { get; set; }
            = null!;


        [JsonIgnore]
        public ParkingSlot ParkingSlot { get; set; }
            = null!;
    }
}