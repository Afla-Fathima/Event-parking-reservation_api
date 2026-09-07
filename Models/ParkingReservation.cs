using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventParkingReservation.Models
{
    public class ParkingReservation
    {
        [Key]
        public int ParkingReservationId { get; set; }

        public int BookingId { get; set; }

        public int ParkingSlotId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Fee { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Active";

        public DateTime ReservedAt { get; set; }
            = DateTime.UtcNow;

        public Booking? Booking { get; set; }

        public ParkingSlot? ParkingSlot { get; set; }
    }
}