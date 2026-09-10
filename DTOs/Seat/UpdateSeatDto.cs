using System.ComponentModel.DataAnnotations;

namespace EventParkingReservation.DTOs.Seat
{
    public class UpdateSeatDto
    {
        [Required]
        [MaxLength(20)]
        public string SeatNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string SeatType { get; set; } = "Regular";

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Available";
    }
}