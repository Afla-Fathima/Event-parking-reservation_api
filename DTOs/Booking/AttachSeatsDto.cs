using System.ComponentModel.DataAnnotations;

namespace EventParkingReservation.DTOs.Booking
{
    public class AttachSeatsDto
    {
        [MinLength(1)]
        public List<int> SeatIds { get; set; }
            = new();
    }
}
