namespace EventParkingReservation.DTOs.Parking
{
    public class ParkingSlotDto
    {
        public int ParkingSlotId { get; set; }

        public int EventId { get; set; }

        public string SlotNumber { get; set; } = string.Empty;

        public string VehicleType { get; set; } = string.Empty;

        public decimal Fee { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
