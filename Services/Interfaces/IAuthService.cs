using EventParkingReservation.DTOs.Auth;
using EventParkingReservation.DTOs.Customer;

namespace EventParkingReservation.Services.Interfaces
{
    public interface IAuthService
    {
        Task<CustomerResponseDto> RegisterAsync(
            RegisterDto dto);

        Task<LoginResponseDto> LoginAsync(
            LoginRequestDto dto);

        Task<bool> CheckEmailAsync(string email);
    }
}
