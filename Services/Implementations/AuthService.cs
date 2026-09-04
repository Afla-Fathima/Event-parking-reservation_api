using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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

        // =====================================================
        // REGISTER
        // =====================================================

        public async Task<CustomerResponseDto> RegisterAsync(
            RegisterDto dto)
        {
            string email =
                dto.Email.Trim().ToLower();

            bool exists =
                await _repository.EmailExistsAsync(email);

            if (exists)
            {
                throw new InvalidOperationException(
                    "Email already exists.");
            }

            var customer = new Customer
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

        // =====================================================
        // LOGIN
        // =====================================================

        public async Task<LoginResponseDto> LoginAsync(
            LoginRequestDto dto)
        {
            string email =
                dto.Email.Trim().ToLower();

            Customer? customer =
                await _repository.GetByEmailAsync(email);

            if (customer == null)
            {
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");
            }

            bool passwordValid =
                BCrypt.Net.BCrypt.Verify(
                    dto.Password,
                    customer.PasswordHash);

            if (!passwordValid)
            {
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");
            }

            if (!string.Equals(
                customer.Status,
                "Active",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException(
                    "Account is inactive.");
            }

            DateTime expiration =
                DateTime.UtcNow.AddHours(2);

            string token =
                GenerateJwtToken(
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

        // =====================================================
        // CHECK EMAIL
        // =====================================================

        public async Task<bool> CheckEmailAsync(
            string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            return await _repository.EmailExistsAsync(
                email.Trim().ToLower());
        }

        // =====================================================
        // FORGOT PASSWORD
        // =====================================================

        public async Task<string> ForgotPasswordAsync(
            ForgotPasswordDto dto)
        {
            string email =
                dto.Email.Trim().ToLower();

            Customer? customer =
                await _repository.GetByEmailAsync(
                    email);

            // Do not reveal whether an account exists.
            if (customer == null)
            {
                return string.Empty;
            }

            byte[] randomBytes =
                RandomNumberGenerator.GetBytes(32);

            string token =
                Convert.ToHexString(randomBytes);

            customer.PasswordResetToken =
                token;

            customer.PasswordResetTokenExpiresAt =
                DateTime.UtcNow.AddMinutes(30);

            await _repository.UpdateAsync(
                customer);

            // Development / student project testing only.
            // In production, send this token by email.
            return token;
        }

        // =====================================================
        // RESET PASSWORD
        // =====================================================

        public async Task ResetPasswordAsync(
            ResetPasswordDto dto)
        {
            Customer? customer =
                await _repository
                    .GetByPasswordResetTokenAsync(
                        dto.Token);

            if (customer == null)
            {
                throw new InvalidOperationException(
                    "Invalid password reset token.");
            }

            if (customer.PasswordResetTokenExpiresAt == null)
            {
                throw new InvalidOperationException(
                    "Invalid password reset token.");
            }

            if (customer.PasswordResetTokenExpiresAt <=
                DateTime.UtcNow)
            {
                customer.PasswordResetToken =
                    null;

                customer.PasswordResetTokenExpiresAt =
                    null;

                await _repository.UpdateAsync(
                    customer);

                throw new InvalidOperationException(
                    "Password reset token has expired.");
            }

            customer.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(
                    dto.NewPassword);

            customer.PasswordResetToken =
                null;

            customer.PasswordResetTokenExpiresAt =
                null;

            await _repository.UpdateAsync(
                customer);
        }

        // =====================================================
        // JWT
        // =====================================================

        private string GenerateJwtToken(
            Customer customer,
            DateTime expiration)
        {
            string jwtKey =
                _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "JWT Key is missing.");

            string jwtIssuer =
                _configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException(
                    "JWT Issuer is missing.");

            string jwtAudience =
                _configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException(
                    "JWT Audience is missing.");

            var claims = new List<Claim>
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

            var key =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        jwtKey));

            var credentials =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256);

            var jwt =
                new JwtSecurityToken(
                    issuer: jwtIssuer,
                    audience: jwtAudience,
                    claims: claims,
                    notBefore: DateTime.UtcNow,
                    expires: expiration,
                    signingCredentials:
                        credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(jwt);
        }

        // =====================================================
        // MAPPING
        // =====================================================

        private static CustomerResponseDto MapCustomer(
            Customer customer)
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