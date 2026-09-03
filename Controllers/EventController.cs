using EventParkingReservation.DTOs.Event;
using EventParkingReservation.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController :
        ControllerBase
    {
        private readonly IEventService _service;

        public EventController(
            IEventService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            string? search = null,
            int? venueId = null,
            int? categoryId = null,
            DateOnly? date = null)
        {
            return Ok(
                await _service.GetAllAsync(
                    search,
                    venueId,
                    categoryId,
                    date));
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
            Create(CreateEventDto dto)
        {
            try
            {
                var result =
                    await _service
                        .CreateAsync(dto);

                return StatusCode(
                    StatusCodes
                        .Status201Created,
                    result);
            }
            catch (Exception ex)
                when (
                    ex is KeyNotFoundException ||
                    ex is InvalidOperationException)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult>
            Update(
                int id,
                UpdateEventDto dto)
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
            catch (InvalidOperationException ex)
            {
                return Conflict(new
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
