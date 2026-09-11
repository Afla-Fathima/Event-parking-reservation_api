using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventParkingReservation.Models
{
    public class Seat
    {
        [Key]
        public int SeatId { get; set; }

        public int EventId { get; set; }

        [Required]
        [MaxLength(20)]
        public string SeatNumber { get; set; }
            = string.Empty;

        [Required]
        [MaxLength(50)]
        public string SeatType { get; set; }
            = "Regular";

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; }
            = "Available";

        public Event? Event { get; set; }

        public ICollection<BookingSeat>
            BookingSeats
        { get; set; }
            = new List<BookingSeat>();
    }
}