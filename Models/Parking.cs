using System.Text.Json.Serialization;

namespace EventParkingReservation.Models
{
    public class ParkingSlot
    {
        public int ParkingSlotId { get; set; }

        public int EventId { get; set; }

        public string SlotNumber { get; set; } = string.Empty;

        public string VehicleType { get; set; } = string.Empty;

        public decimal Fee { get; set; }

        public string Status { get; set; } = "Available";

        [JsonIgnore]
        public Event Event { get; set; } = null!;

        [JsonIgnore]
        public ICollection<ParkingReservation> ParkingReservations
        {
            get;
            set;
        } = new List<ParkingReservation>();
    }
}