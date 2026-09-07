using System.ComponentModel.DataAnnotations;

namespace EventParkingReservation.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = "Customer";

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Active";

        // =====================================================
        // EMAIL VERIFICATION
        // =====================================================

        public bool EmailVerified { get; set; } = false;

        public string? EmailVerificationToken { get; set; }

        public DateTime? EmailVerificationTokenExpiresAt { get; set; }

        // =====================================================
        // PASSWORD RESET
        // =====================================================

        public string? PasswordResetToken { get; set; }

        public DateTime? PasswordResetTokenExpiresAt { get; set; }

        // =====================================================
        // AUDIT
        // =====================================================

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // =====================================================
        // RELATIONSHIPS
        // =====================================================

        public ICollection<Booking> Bookings { get; set; }
            = new List<Booking>();

        public ICollection<Notification> Notifications { get; set; }
            = new List<Notification>();
    }
}