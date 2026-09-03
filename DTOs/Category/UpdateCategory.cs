namespace EventParkingReservation.DTOs.Category
{
    public class UpdateCategoryDto : CreateCategoryDto
    {
        public string Status { get; set; } = "Active";
    }
}
