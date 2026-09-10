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

        // ADMIN + CUSTOMER
        // GET api/events/1/seats
        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetMap(
            int eventId)
        {
            try
            {
                var seats =
                    await _service
                        .GetByEventIdAsync(eventId);

                return Ok(seats);
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
        // POST api/events/1/seats
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(
            int eventId,
            [FromBody] CreateSeatDto dto)
        {
            try
            {
                var result =
                    await _service.CreateAsync(
                        eventId,
                        dto
                    );

                return Created(
                    $"/api/events/{eventId}/seats/{result.SeatId}",
                    result
                );
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

        // ADMIN ONLY
        // ONE CLICK -> GENERATE 200 SEATS
        // POST api/events/1/seats/generate
        [HttpPost("generate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Generate(
            int eventId)
        {
            try
            {
                var createdCount =
                    await _service
                        .GenerateDefaultSeatsAsync(
                            eventId
                        );

                return Ok(new
                {
                    message =
                        $"{createdCount} seats generated successfully.",

                    createdSeats =
                        createdCount,

                    maximumSeats =
                        200
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
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        // ADMIN ONLY
        // PUT api/events/1/seats/5
        [HttpPut("{seatId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(
            int eventId,
            int seatId,
            [FromBody] UpdateSeatDto dto)
        {
            try
            {
                var result =
                    await _service.UpdateAsync(
                        eventId,
                        seatId,
                        dto
                    );

                return Ok(result);
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

        // ADMIN ONLY
        // DELETE api/events/1/seats/5
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
                    seatId
                );

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