using EventParkingReservation.DTOs.Auth;
using EventParkingReservation.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservation.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;
        private readonly IWebHostEnvironment _environment;

        public AuthController(
            IAuthService service,
            IWebHostEnvironment environment)
        {
            _service = service;
            _environment = environment;
        }

        // CUSTOMER REGISTRATION
        // BRD endpoint:
        // POST /api/customers/register
        [HttpPost("~/api/customers/register")]
        public async Task<IActionResult> Register(
            RegisterDto dto)
        {
            var result =
                await _service.RegisterAsync(dto);

            return StatusCode(
                StatusCodes.Status201Created,
                result);
        }

        // LOGIN
        // POST /api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            LoginRequestDto dto)
        {
            return Ok(
                await _service.LoginAsync(dto));
        }

        // CHECK EMAIL
        // GET /api/auth/check-email
        [HttpGet("check-email")]
        public async Task<IActionResult> CheckEmail(
            [FromQuery] string email)
        {
            return Ok(new
            {
                exists =
                    await _service.CheckEmailAsync(email)
            });
        }

        // FORGOT PASSWORD
        // POST /api/auth/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(
            ForgotPasswordDto dto)
        {
            string token =
                await _service.ForgotPasswordAsync(dto);

            if (_environment.IsDevelopment())
            {
                return Ok(new
                {
                    message =
                        "If the account exists, a password reset request was created.",

                    resetToken = token
                });
            }

            return Ok(new
            {
                message =
                    "If the account exists, a password reset request was created."
            });
        }

        // RESET PASSWORD
        // POST /api/auth/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
            ResetPasswordDto dto)
        {
            await _service.ResetPasswordAsync(dto);

            return Ok(new
            {
                message =
                    "Password reset successfully."
            });
        }
    }
}