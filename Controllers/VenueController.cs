using EventParkingReservation.DTOs.Venue;
using EventParkingReservation.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VenueController :
        ControllerBase
    {
        private readonly IVenueService _service;

        public VenueController(
            IVenueService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(
                await _service.GetAllAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult>
            GetById(int id)
        {
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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult>
            Create(CreateVenueDto dto)
        {
            var result =
                await _service.CreateAsync(dto);

            return StatusCode(
                StatusCodes.Status201Created,
                result);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult>
            Update(
                int id,
                UpdateVenueDto dto)
        {
            try
            {
                return Ok(
                    await _service
                        .UpdateAsync(
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
            Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);

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
    }
}
