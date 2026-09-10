using EventParkingReservation.DTOs.Seat;
using EventParkingReservation.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservation.Controllers
{
    [ApiController]
    [Route(
        "api/events/{eventId:int}/seats")]
    [Authorize]
    public class SeatController :
        ControllerBase
    {
        private readonly ISeatService
            _service;

        public SeatController(
            ISeatService service)
        {
            _service =
                service;
        }

        // =====================================================
        // GET ALL EVENT SEATS
        //
        // Admin + Customer
        //
        // GET:
        // /api/events/1/seats
        // =====================================================

        [HttpGet]
        [Authorize(
            Roles =
                "Admin,Customer")]
        public async Task<IActionResult>
            GetMap(
                int eventId)
        {
            try
            {
                var result =
                    await _service
                        .GetByEventIdAsync(
                            eventId);

                return Ok(
                    result);
            }
            catch (
                KeyNotFoundException ex)
            {
                return NotFound(
                    new
                    {
                        message =
                            ex.Message
                    });
            }
        }

        // =====================================================
        // CREATE SINGLE SEAT
        //
        // ADMIN ONLY
        //
        // POST:
        // /api/events/1/seats
        // =====================================================

        [HttpPost]
        [Authorize(
            Roles =
                "Admin")]
        public async Task<IActionResult>
            Create(
                int eventId,
                [FromBody]
                CreateSeatDto dto)
        {
            try
            {
                var result =
                    await _service
                        .CreateAsync(
                            eventId,
                            dto);

                return Created(
                    $"/api/events/{eventId}/seats/{result.SeatId}",
                    result);
            }
            catch (
                KeyNotFoundException ex)
            {
                return NotFound(
                    new
                    {
                        message =
                            ex.Message
                    });
            }
            catch (
                InvalidOperationException ex)
            {
                return Conflict(
                    new
                    {
                        message =
                            ex.Message
                    });
            }
        }

        // =====================================================
        // ? GENERATE 200 SEATS
        //
        // ADMIN ONLY
        //
        // POST:
        // /api/events/1/seats/generate
        //
        // NO BODY REQUIRED
        //
        // =====================================================

        [HttpPost("generate")]
        [Authorize(
            Roles =
                "Admin")]
        public async Task<IActionResult>
            Generate(
                int eventId)
        {
            try
            {
                var createdCount =
                    await _service
                        .GenerateDefaultSeatsAsync(
                            eventId);

                var totalSeats =
                    await _service
                        .GetByEventIdAsync(
                            eventId);

                return Ok(
                    new
                    {
                        message =
                            $"{createdCount} seats generated successfully.",

                        createdSeats =
                            createdCount,

                        totalSeats =
                            totalSeats.Count,

                        requiredSeatMap =
                            200
                    });
            }
            catch (
                KeyNotFoundException ex)
            {
                return NotFound(
                    new
                    {
                        message =
                            ex.Message
                    });
            }
            catch (
                InvalidOperationException ex)
            {
                return Conflict(
                    new
                    {
                        message =
                            ex.Message
                    });
            }
        }

        // =====================================================
        // UPDATE SEAT
        //
        // ADMIN ONLY
        //
        // PUT:
        // /api/events/1/seats/5
        // =====================================================

        [HttpPut("{seatId:int}")]
        [Authorize(
            Roles =
                "Admin")]
        public async Task<IActionResult>
            Update(
                int eventId,
                int seatId,
                [FromBody]
                UpdateSeatDto dto)
        {
            try
            {
                var result =
                    await _service
                        .UpdateAsync(
                            eventId,
                            seatId,
                            dto);

                return Ok(
                    result);
            }
            catch (
                KeyNotFoundException ex)
            {
                return NotFound(
                    new
                    {
                        message =
                            ex.Message
                    });
            }
            catch (
                InvalidOperationException ex)
            {
                return Conflict(
                    new
                    {
                        message =
                            ex.Message
                    });
            }
        }

        // =====================================================
        // DELETE SEAT
        //
        // ADMIN ONLY
        //
        // DELETE:
        // /api/events/1/seats/5
        // =====================================================

        [HttpDelete("{seatId:int}")]
        [Authorize(
            Roles =
                "Admin")]
        public async Task<IActionResult>
            Delete(
                int eventId,
                int seatId)
        {
            try
            {
                await _service
                    .DeleteAsync(
                        eventId,
                        seatId);

                return NoContent();
            }
            catch (
                KeyNotFoundException ex)
            {
                return NotFound(
                    new
                    {
                        message =
                            ex.Message
                    });
            }
            catch (
                InvalidOperationException ex)
            {
                return Conflict(
                    new
                    {
                        message =
                            ex.Message
                    });
            }
        }
    }
}