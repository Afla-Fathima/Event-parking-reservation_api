using System.ComponentModel.DataAnnotations;

namespace EventParkingReservation.DTOs.Seat
{
    public class CreateSeatDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int EventId { get; set; }

        [Required]
        [StringLength(20)]
        public string SeatNumber { get; set; }
            = string.Empty;

        [Required]
        [StringLength(50)]
        public string SeatType { get; set; }
            = string.Empty;

        [Required]
        [Range(0.01, 1000000)]
        public decimal Price { get; set; }
    }
}