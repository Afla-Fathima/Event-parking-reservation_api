using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EventParkingReservation.DTOs.Auth;
using EventParkingReservation.DTOs.Customer;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Interfaces;
using EventParkingReservation.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace EventParkingReservation.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repository;
        private readonly IConfiguration _configuration;

        public AuthService(
            IAuthRepository repository,
            IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        public async Task<CustomerResponseDto>
            RegisterAsync(RegisterDto dto)
        {
            string email =
                dto.Email.Trim().ToLower();

            if (await _repository
                .EmailExistsAsync(email))
            {
                throw new InvalidOperationException(
                    "Email already exists.");
            }

            Customer customer = new()
            {
                Name = dto.FullName.Trim(),
                Email = email,
                PhoneNumber = dto.Phone.Trim(),
                PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(
                        dto.Password),
                Role = "Customer",
                Status = "Active",
                CreatedDate = DateTime.UtcNow
            };

            await _repository.AddAsync(customer);

            return MapCustomer(customer);
        }

        public async Task<LoginResponseDto>
            LoginAsync(LoginRequestDto dto)
        {
            string email =
                dto.Email.Trim().ToLower();

            Customer? customer =
                await _repository
                    .GetByEmailAsync(email);

            if (customer == null)
            {
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");
            }

            bool valid =
                BCrypt.Net.BCrypt.Verify(
                    dto.Password,
                    customer.PasswordHash);

            if (!valid)
            {
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");
            }

            if (customer.Status != "Active")
            {
                throw new UnauthorizedAccessException(
                    "Account is inactive.");
            }

            DateTime expiration =
                DateTime.UtcNow.AddHours(2);

            string token =
                GenerateToken(
                    customer,
                    expiration);

            return new LoginResponseDto
            {
                CustomerId =
                    customer.CustomerId,

                FullName =
                    customer.Name,

                Email =
                    customer.Email,

                Role =
                    customer.Role,

                Token =
                    token,

                Expiration =
                    expiration
            };
        }

        public async Task<bool> CheckEmailAsync(
            string email)
        {
            return await _repository
                .EmailExistsAsync(
                    email.Trim().ToLower());
        }

        private string GenerateToken(
            Customer customer,
            DateTime expiration)
        {
            string key =
                _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "JWT Key is missing.");

            string issuer =
                _configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException(
                    "JWT Issuer is missing.");

            string audience =
                _configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException(
                    "JWT Audience is missing.");

            Claim[] claims =
            {
                new(
                    ClaimTypes.NameIdentifier,
                    customer.CustomerId.ToString()),

                new(
                    ClaimTypes.Name,
                    customer.Name),

                new(
                    ClaimTypes.Email,
                    customer.Email),

                new(
                    ClaimTypes.Role,
                    customer.Role)
            };

            SymmetricSecurityKey securityKey =
                new(
                    Encoding.UTF8.GetBytes(key));

            SigningCredentials credentials =
                new(
                    securityKey,
                    SecurityAlgorithms
                        .HmacSha256);

            JwtSecurityToken token =
                new(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: expiration,
                    signingCredentials:
                        credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        private static CustomerResponseDto
            MapCustomer(Customer customer)
        {
            return new CustomerResponseDto
            {
                CustomerId =
                    customer.CustomerId,

                Name =
                    customer.Name,

                Email =
                    customer.Email,

                PhoneNumber =
                    customer.PhoneNumber,

                Role =
                    customer.Role,

                Status =
                    customer.Status
            };
        }
    }
}
