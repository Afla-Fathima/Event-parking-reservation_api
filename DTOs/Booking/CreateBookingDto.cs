using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EventParkingReservation.DTOs.Booking
{
    public class CreateBookingDto
    {
        // Client cannot send/change this.
        // BookingController fills this from JWT.
        [JsonIgnore]
        public int CustomerId { get; set; }

        [Range(1, int.MaxValue)]
        public int EventId { get; set; }

        [Required]
        [MinLength(1)]
        public List<int> SeatIds { get; set; } = new();

        public int? ParkingSlotId { get; set; }
    }
}