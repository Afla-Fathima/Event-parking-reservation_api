using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventParkingReservation.Models
{
    public class ParkingSlot
    {
        [Key]
        public int ParkingSlotId { get; set; }

        public int EventId { get; set; }

        [Required]
        [MaxLength(20)]
        public string SlotNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string VehicleType { get; set; } = "Car";

        [Column(TypeName = "decimal(18,2)")]
        public decimal Fee { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Available";

        public Event? Event { get; set; }

        public ICollection<ParkingReservation> ParkingReservations
        {
            get;
            set;
        } = new List<ParkingReservation>();
    }
}
