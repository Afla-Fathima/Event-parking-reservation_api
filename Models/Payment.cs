using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventParkingReservation.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        public int BookingId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        public string PaymentMethod { get; set; } = "Simulation";

        [MaxLength(30)]
        public string Status { get; set; } = "Completed";

        [MaxLength(100)]
        public string TransactionReference { get; set; } = string.Empty;

        public Booking? Booking { get; set; }
    }
}
