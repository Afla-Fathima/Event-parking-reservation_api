using System.ComponentModel.DataAnnotations;

namespace EventParkingReservation.DTOs.ParkingSlot
{
    public class UpdateParkingSlotDto
    {
        [Required]
        [StringLength(20)]
        public string SlotNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string VehicleType { get; set; } = string.Empty;

        [Required]
        [Range(0, 100000)]
        public decimal Fee { get; set; }
    }
}