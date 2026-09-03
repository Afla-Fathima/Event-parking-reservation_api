using EventParkingReservation.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventParkingReservation.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Required]
        [MaxLength(50)]
        public string BookingNumber { get; set; } = string.Empty;

        public int CustomerId { get; set; }

        public int EventId { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "Pending";

        [Required]
        [MaxLength(30)]
        public string PaymentStatus { get; set; } = "Pending";

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public Customer? Customer { get; set; }

        public Event? Event { get; set; }

        public ICollection<BookingSeat> BookingSeats { get; set; }
            = new List<BookingSeat>();

        public ParkingReservation? ParkingReservation { get; set; }

        public Payment? Payment { get; set; }
    }
}
