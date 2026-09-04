using EventParkingReservation.DTOs.Parking;
using EventParkingReservation.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservation.Controllers
{
    [ApiController]
    [Route("api/events/{eventId:int}/parking-slots")]
    [Authorize]
    public class ParkingController : ControllerBase
    {
        private readonly IParkingService _service;

        public ParkingController(
            IParkingService service)
        {
            _service = service;
        }

        // Customer + Admin view
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
            CreateParkingSlotDto dto)
        {
            try
            {
                var result =
                    await _service.CreateAsync(
                        eventId,
                        dto);

                return Created(
                    $"/api/events/{eventId}/parking-slots/{result.ParkingSlotId}",
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

        [HttpPut("{slotId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(
            int eventId,
            int slotId,
            UpdateParkingSlotDto dto)
        {
            try
            {
                return Ok(
                    await _service.UpdateAsync(
                        eventId,
                        slotId,
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

        [HttpDelete("{slotId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(
            int eventId,
            int slotId)
        {
            try
            {
                await _service.DeleteAsync(
                    eventId,
                    slotId);

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