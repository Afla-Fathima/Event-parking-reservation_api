using System.ComponentModel.DataAnnotations;

namespace EventParkingReservation.DTOs.Seat
{
    public class UpdateSeatDto
    {
        [Required]
        [StringLength(20)]
        public string SeatNumber { get; set; }
            = string.Empty;

        [Required]
        [StringLength(50)]
        public string SeatType { get; set; }
            = "Regular";

        [Required]
        [Range(0, 1000000)]
        public decimal Price { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; }
            = "Available";
    }
}