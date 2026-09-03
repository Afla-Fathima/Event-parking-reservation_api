namespace EventParkingReservation.DTOs.Customer
{
    public class CustomerResponseDto
    {
        public int CustomerId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
    }
}
