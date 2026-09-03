using System.ComponentModel.DataAnnotations;

namespace EventParkingReservation.DTOs.Parking
{
    public class CreateParkingSlotDto
    {
        [Required]
        public string SlotNumber { get; set; } = string.Empty;

        [Required]
        public string VehicleType { get; set; } = "Car";

        [Range(0, double.MaxValue)]
        public decimal Fee { get; set; }
    }
}
