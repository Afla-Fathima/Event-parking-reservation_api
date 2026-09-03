using EventParkingReservation.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController :
        ControllerBase
    {
        private readonly IDashboardService
            _service;

        public DashboardController(
            IDashboardService service)
        {
            _service = service;
        }

        [HttpGet(
            "customer/{customerId:int}")]
        public async Task<IActionResult>
            CustomerDashboard(
                int customerId)
        {
            return Ok(
                await _service
                    .GetCustomerDashboardAsync(
                        customerId));
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult>
            AdminDashboard()
        {
            return Ok(
                await _service
                    .GetAdminDashboardAsync());
        }
    }
}
