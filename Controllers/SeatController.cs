using EventParkingReservation.DTOs.Seat;
using EventParkingReservation.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservation.Controllers
{
    [ApiController]
    [Route("api/events/{eventId:int}/seats")]
    [Authorize]
    public class SeatController : ControllerBase
    {
        private readonly ISeatService _service;

        public SeatController(
            ISeatService service)
        {
            _service = service;
        }

        // Admin + Customer can see seat map
        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetMap(
            int eventId)
        {
            try
            {
                return Ok(
                    await _service.GetByEventIdAsync(eventId));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        // ADMIN ONLY
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(
            int eventId,
            CreateSeatDto dto)
        {
            try
            {
                var result =
                    await _service.CreateAsync(
                        eventId,
                        dto);

                return Created(
                    $"/api/events/{eventId}/seats/{result.SeatId}",
                    result);
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

        [HttpPut("{seatId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(
            int eventId,
            int seatId,
            UpdateSeatDto dto)
        {
            try
            {
                return Ok(
                    await _service.UpdateAsync(
                        eventId,
                        seatId,
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

        [HttpDelete("{seatId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(
            int eventId,
            int seatId)
        {
            try
            {
                await _service.DeleteAsync(
                    eventId,
                    seatId);

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