using System.ComponentModel.DataAnnotations;

namespace EventParkingReservation.DTOs.Seat
{
    public class CreateSeatDto
    {
        [Required]
        public string SeatNumber { get; set; } = string.Empty;

        [Required]
        public string SeatType { get; set; } = "Regular";

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
    }
}
