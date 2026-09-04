using EventParkingReservation.DTOs.Customer;
using EventParkingReservation.Security;
using EventParkingReservation.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservation.Controllers
{
    [ApiController]
    [Route("api/customers")]
    [Authorize]
    public class CustomerController :
        ControllerBase
    {
        private readonly ICustomerService _service;

        public CustomerController(
            ICustomerService service)
        {
            _service = service;
        }

        [HttpGet("me")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Me()
        {
            return Ok(
                await _service.GetByIdAsync(
                    User.GetCustomerId()));
        }

        [HttpPut("me")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateMe(
            UpdateCustomerDto dto)
        {
            return Ok(
                await _service.UpdateAsync(
                    User.GetCustomerId(),
                    dto));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search)
        {
            return Ok(
                await _service.GetAllAsync(
                    search));
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetById(
            int id)
        {
            if (User.IsInRole("Customer") &&
                User.GetCustomerId() != id)
            {
                return Forbid();
            }

            try
            {
                return Ok(
                    await _service
                        .GetByIdAsync(id));
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
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Update(
            int id,
            UpdateCustomerDto dto)
        {
            if (User.GetCustomerId() != id)
            {
                return Forbid();
            }

            try
            {
                return Ok(
                    await _service.UpdateAsync(
                        id,
                        dto));
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
        public async Task<IActionResult>
            Deactivate(
                int id)
        {
            try
            {
                await _service
                    .DeactivateAsync(id);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id:int}/reactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult>
            Reactivate(
                int id)
        {
            try
            {
                await _service
                    .ReactivateAsync(id);

                return NoContent();
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