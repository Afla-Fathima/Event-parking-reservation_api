
using EventParkingReservation.DTOs.Seat;
using EventParkingReservation.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatController : ControllerBase
    {
        private readonly ISeatService _service;

        public SeatController(
            ISeatService service)
        {
            _service = service;
        }


        // ==========================================
        // GET: api/Seat/event/1
        // ==========================================

        [HttpGet("event/{eventId:int}")]
        public async Task<IActionResult>
            GetByEvent(int eventId)
        {
            var seats =
                await _service
                    .GetByEventIdAsync(eventId);

            return Ok(seats);
        }


        // ==========================================
        // GET: api/Seat/1
        // ==========================================

        [HttpGet("{id:int}")]
        public async Task<IActionResult>
            GetById(int id)
        {
            var seat =
                await _service
                    .GetByIdAsync(id);

            if (seat == null)
            {
                return NotFound(new
                {
                    message = "Seat not found"
                });
            }

            return Ok(seat);
        }


        // ==========================================
        // POST: api/Seat
        // Admin Only
        // ==========================================

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult>
            Create(CreateSeatDto dto)
        {
            try
            {
                var seat =
                    await _service
                        .AddAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new
                    {
                        id = seat.SeatId
                    },
                    seat);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }


        // ==========================================
        // PUT: api/Seat/1
        // Admin Only
        // ==========================================

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult>
            Update(
                int id,
                UpdateSeatDto dto)
        {
            try
            {
                var seat =
                    await _service
                        .UpdateAsync(id, dto);

                return Ok(new
                {
                    message =
                        "Seat updated successfully",

                    data = seat
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


        // ==========================================
        // DELETE: api/Seat/1
        // Admin Only
        // ==========================================

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult>
            Delete(int id)
        {
            try
            {
                await _service
                    .DeleteAsync(id);

                return Ok(new
                {
                    message =
                        "Seat deleted successfully"
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