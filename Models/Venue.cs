using EventParkingReservation.Models;
using System.ComponentModel.DataAnnotations;

namespace EventParkingReservation.Models
{
    public class Venue
    {
        [Key]
        public int VenueId { get; set; }

        [Required]
        [MaxLength(150)]
        public string VenueName { get; set; } = string.Empty;

        [Required]
        [MaxLength(250)]
        public string Location { get; set; } = string.Empty;

        public int Capacity { get; set; }

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public ICollection<Event> Events { get; set; }
            = new List<Event>();
    }
}
