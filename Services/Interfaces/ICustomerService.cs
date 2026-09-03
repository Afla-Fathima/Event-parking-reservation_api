using EventParkingReservation.DTOs.Customer;

namespace EventParkingReservation.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerResponseDto>> GetAllAsync(
            string? search);

        Task<CustomerResponseDto> GetByIdAsync(
            int id);

        Task<CustomerResponseDto> UpdateAsync(
            int id,
            UpdateCustomerDto dto);

        Task DeactivateAsync(int id);

        Task ReactivateAsync(int id);
    }
}