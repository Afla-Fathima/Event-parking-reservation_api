using EventParkingReservation.DTOs.Customer;
using EventParkingReservation.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _service;

        public CustomerController(
            ICustomerService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll(
            string? search = null)
        {
            var result =
                await _service.GetAllAsync(
                    search);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(
            int id)
        {
            try
            {
                var result =
                    await _service.GetByIdAsync(
                        id);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateCustomerDto dto)
        {
            try
            {
                var result =
                    await _service.UpdateAsync(
                        id,
                        dto);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Deactivate(
            int id)
        {
            try
            {
                await _service.DeactivateAsync(
                    id);

                return Ok(new
                {
                    message =
                        "Customer deactivated successfully."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id:int}/reactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reactivate(
            int id)
        {
            try
            {
                await _service.ReactivateAsync(
                    id);

                return Ok(new
                {
                    message =
                        "Customer reactivated successfully."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }
    }
}