using System.ComponentModel.DataAnnotations;

namespace EventParkingReservation.DTOs.Payment
{
    public class CreatePaymentDto
    {
        [Required]
        public string PaymentMethod { get; set; }
            = "Simulation";
    }
}
