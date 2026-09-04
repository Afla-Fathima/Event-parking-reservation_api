using EventParkingReservation.DTOs.Event;
using EventParkingReservation.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservation.Controllers
{
    [ApiController]
    [Route("api/events")]
    [Authorize]
    public class EventController : ControllerBase
    {
        private readonly IEventService _service;

        public EventController(
            IEventService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] DateOnly? date,
            [FromQuery] int? venue,
            [FromQuery] int? category)
        {
            var result =
                await _service.GetAllAsync(
                    search,
                    date,
                    venue,
                    category);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetById(
            int id)
        {
            var result =
                await _service.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(
            CreateEventDto dto)
        {
            try
            {
                var result =
                    await _service.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = result.EventId },
                    result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(
            int id,
            UpdateEventDto dto)
        {
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
        public async Task<IActionResult> Delete(
            int id)
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