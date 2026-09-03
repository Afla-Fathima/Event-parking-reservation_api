using EventParkingReservation.DTOs.ParkingSlot;
using EventParkingReservation.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParkingSlotController : ControllerBase
    {
        private readonly IParkingSlotService _service;

        public ParkingSlotController(
            IParkingSlotService service)
        {
            _service = service;
        }

        // GET: api/ParkingSlot
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var slots =
                await _service.GetAllAsync();

            return Ok(slots);
        }

        // GET: api/ParkingSlot/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult>
            GetById(int id)
        {
            var slot =
                await _service.GetByIdAsync(id);

            if (slot == null)
            {
                return NotFound(new
                {
                    message =
                        "Parking slot not found"
                });
            }

            return Ok(slot);
        }

        // GET: api/ParkingSlot/event/1
        [HttpGet("event/{eventId:int}")]
        public async Task<IActionResult>
            GetByEvent(int eventId)
        {
            var slots =
                await _service
                    .GetByEventIdAsync(eventId);

            return Ok(slots);
        }

        // POST: api/ParkingSlot
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult>
            Create(CreateParkingSlotDto dto)
        {
            try
            {
                var slot =
                    await _service.AddAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new
                    {
                        id = slot.ParkingSlotId
                    },
                    slot);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // PUT: api/ParkingSlot/1
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult>
            Update(
                int id,
                UpdateParkingSlotDto dto)
        {
            try
            {
                var slot =
                    await _service
                        .UpdateAsync(id, dto);

                return Ok(new
                {
                    message =
                        "Parking slot updated successfully",

                    data = slot
                });
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
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // DELETE: api/ParkingSlot/1
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult>
            Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);

                return Ok(new
                {
                    message =
                        "Parking slot deleted successfully"
                });
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
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}