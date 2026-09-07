using System.ComponentModel.DataAnnotations;

namespace EventParkingReservation.DTOs.Event
{
    public class CreateEventDto
    {
        [Required]
        public string EventName { get; set; } = string.Empty;

        public DateOnly EventDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public string Description { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal TicketPrice { get; set; }

        [Range(0, double.MaxValue)]
        public decimal ParkingFee { get; set; }

        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }

        [Range(1, int.MaxValue)]
        public int VenueId { get; set; }

        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }
    }
}
