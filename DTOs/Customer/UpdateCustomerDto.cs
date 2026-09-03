using System.ComponentModel.DataAnnotations;

namespace EventParkingReservation.DTOs.Customer
{
    public class UpdateCustomerDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
