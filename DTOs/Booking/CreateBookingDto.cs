using System.ComponentModel.DataAnnotations;

namespace EventParkingReservation.DTOs.Booking
{
    public class CreateBookingDto
    {
        [Range(1, int.MaxValue)]
        public int CustomerId { get; set; }

        [Range(1, int.MaxValue)]
        public int EventId { get; set; }

        [MinLength(1)]
        public List<int> SeatIds { get; set; }
            = new List<int>();

        public int? ParkingSlotId { get; set; }
    }
}
