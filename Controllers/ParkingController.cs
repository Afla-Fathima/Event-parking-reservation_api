using EventParkingReservation.DTOs.Parking;
using EventParkingReservation.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservation.Controllers
{
    [ApiController]

    [Route(
        "api/events/{eventId:int}/parking-slots")]

    [Authorize]
    public class ParkingController
        : ControllerBase
    {
        private readonly IParkingService
            _service;


        public ParkingController(
            IParkingService service)
        {
            _service =
                service;
        }


        // ==========================================
        // ADMIN + CUSTOMER
        //
        // GET
        // /api/events/1/parking-slots
        // ==========================================

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
                return Ok(
                    await _service
                        .GetByEventIdAsync(
                            eventId));
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


        // ==========================================
        // ADMIN
        //
        // CREATE ONE
        //
        // POST
        // /api/events/1/parking-slots
        // ==========================================

        [HttpPost]

        [Authorize(
            Roles =
                "Admin")]

        public async Task<IActionResult>
            Create(
                int eventId,

                [FromBody]
                CreateParkingSlotDto dto)
        {
            try
            {
                var result =
                    await _service
                        .CreateAsync(
                            eventId,
                            dto);


                return Created(
                    $"/api/events/{eventId}/parking-slots/{result.ParkingSlotId}",
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


        // ==========================================
        // ? ADMIN
        //
        // GENERATE UNTIL TOTAL = 20
        //
        // POST
        // /api/events/1/parking-slots/generate
        //
        // NO JSON BODY
        // ==========================================

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
                var created =
                    await _service
                        .GenerateDefaultSlotsAsync(
                            eventId);


                var allSlots =
                    await _service
                        .GetByEventIdAsync(
                            eventId);


                return Ok(
                    new
                    {
                        message =
                            $"{created} parking slots generated successfully.",

                        createdSlots =
                            created,

                        totalSlots =
                            allSlots.Count,

                        requiredSlots =
                            20
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


        // ==========================================
        // ADMIN UPDATE
        // ==========================================

        [HttpPut(
            "{slotId:int}")]

        [Authorize(
            Roles =
                "Admin")]

        public async Task<IActionResult>
            Update(
                int eventId,
                int slotId,

                [FromBody]
                UpdateParkingSlotDto dto)
        {
            try
            {
                return Ok(
                    await _service
                        .UpdateAsync(
                            eventId,
                            slotId,
                            dto));
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


        // ==========================================
        // ADMIN DELETE
        // ==========================================

        [HttpDelete(
            "{slotId:int}")]

        [Authorize(
            Roles =
                "Admin")]

        public async Task<IActionResult>
            Delete(
                int eventId,
                int slotId)
        {
            try
            {
                await _service
                    .DeleteAsync(
                        eventId,
                        slotId);


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