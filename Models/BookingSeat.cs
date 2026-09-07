using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventParkingReservation.Models
{
    public class BookingSeat
    {
        [Key]
        public int BookingSeatId { get; set; }

        public int BookingId { get; set; }

        public int SeatId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SeatPrice { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Active";

        public Booking? Booking { get; set; }

        public Seat? Seat { get; set; }
    }
}
