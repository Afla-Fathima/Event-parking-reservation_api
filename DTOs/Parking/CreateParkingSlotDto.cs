using System.ComponentModel.DataAnnotations;

namespace EventParkingReservation.DTOs.ParkingSlot
{
    public class CreateParkingSlotDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int EventId { get; set; }

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