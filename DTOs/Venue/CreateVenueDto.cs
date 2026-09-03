using System.ComponentModel.DataAnnotations;

namespace EventParkingReservation.DTOs.Venue
{
    public class CreateVenueDto
    {
        [Required]
        public string VenueName { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}
