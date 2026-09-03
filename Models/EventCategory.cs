using EventParkingReservation.Models;
using System.ComponentModel.DataAnnotations;

namespace EventParkingReservation.Models
{
    public class EventCategory
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Status { get; set; } = "Active";

        public ICollection<Event> Events { get; set; }
            = new List<Event>();
    }
}
