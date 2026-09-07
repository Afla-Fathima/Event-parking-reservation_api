using System.ComponentModel.DataAnnotations;

namespace EventParkingReservation.DTOs.Category
{
    public class CreateCategoryDto
    {
        [Required]
        public string CategoryName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}
