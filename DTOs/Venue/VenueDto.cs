namespace EventParkingReservation.DTOs.Venue
{
    public class VenueDto
    {
        public int VenueId { get; set; }

        public string VenueName { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}
