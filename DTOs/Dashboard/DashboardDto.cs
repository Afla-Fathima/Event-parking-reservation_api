namespace EventParkingReservation.DTOs.Dashboard
{
    public class CustomerDashboardDto
    {
        public int UpcomingBookings { get; set; }

        public int ReservedParking { get; set; }

        public int RecentPayments { get; set; }

        public int UnreadNotifications { get; set; }
    }

    public class AdminDashboardDto
    {
        public int TotalEvents { get; set; }

        public int TotalBookings { get; set; }

        public int AvailableSeats { get; set; }

        public int OccupiedParkingSlots { get; set; }

        public decimal TotalRevenue { get; set; }

        public int TotalCustomers { get; set; }
    }
}
