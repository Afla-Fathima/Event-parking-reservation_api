using System.ComponentModel.DataAnnotations;

namespace EventParkingReservation.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        public int CustomerId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Message { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Type { get; set; } = "Info";

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Customer? Customer { get; set; }
    }
}
