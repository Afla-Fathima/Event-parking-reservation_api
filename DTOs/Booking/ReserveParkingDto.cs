using System.ComponentModel.DataAnnotations;

namespace EventParkingReservation.DTOs.Booking
{
    public class ReserveParkingDto
    {
        [Range(1, int.MaxValue)]
        public int ParkingSlotId { get; set; }
    }
}
