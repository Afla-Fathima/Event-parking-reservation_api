namespace EventParkingReservation.DTOs.Event
{
    public class EventDto
    {
        public int EventId { get; set; }

        public string EventName { get; set; } = string.Empty;

        public DateOnly EventDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public string Description { get; set; } = string.Empty;

        public decimal TicketPrice { get; set; }

        public decimal ParkingFee { get; set; }

        public int Capacity { get; set; }

        public int VenueId { get; set; }

        public string VenueName { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;
    }
}
