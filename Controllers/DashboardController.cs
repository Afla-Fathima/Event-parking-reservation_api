using EventParkingReservation.Security;
using EventParkingReservation.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservation.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _service;

        public DashboardController(
            IDashboardService service)
        {
            _service = service;
        }

        [HttpGet("customer")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Customer()
        {
            var customerId =
                User.GetCustomerId();

            return Ok(
                await _service.GetCustomerAsync(
                    customerId));
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Admin()
        {
            return Ok(
                await _service.GetAdminAsync());
        }
    }
}