using EventParkingReservation.DTOs.Auth;
using EventParkingReservation.Services.Interfaces;
using EventParkingReservation.DTOs.Auth;
using EventParkingReservation.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace EventParkingReservation.Controllers;
[ApiController, Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;
    public AuthController(IAuthService service) => _service = service;
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto) { try { var id = await _service.RegisterAsync(dto); return Created($"/api/customers/{id}", new { customerId = id, message = "Registration successful." }); } catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); } }
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto dto) { try { return Ok(await _service.LoginAsync(dto)); } catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); } }
}
