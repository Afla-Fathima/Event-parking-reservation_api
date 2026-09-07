using EventParkingReservation.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventParkingReservation.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required]
        [MaxLength(200)]
        public string EventName { get; set; } = string.Empty;

        public DateOnly EventDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TicketPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ParkingFee { get; set; }

        public int Capacity { get; set; }

        public int VenueId { get; set; }

        public int CategoryId { get; set; }

        public Venue? Venue { get; set; }

        public EventCategory? Category { get; set; }

        public ICollection<Seat> Seats { get; set; }
            = new List<Seat>();

        public ICollection<ParkingSlot> ParkingSlots { get; set; }
            = new List<ParkingSlot>();

        public ICollection<Booking> Bookings { get; set; }
            = new List<Booking>();
    }
}
