using EventParkingReservation.DTOs.Booking;
using EventParkingReservation.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingController :
        ControllerBase
    {
        private readonly IBookingService
            _service;

        public BookingController(
            IBookingService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult>
            GetAll(int? eventId = null)
        {
            return Ok(
                await _service
                    .GetAllAsync(eventId));
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

        [HttpGet(
            "customer/{customerId:int}")]
        public async Task<IActionResult>
            GetByCustomer(
                int customerId)
        {
            return Ok(
                await _service
                    .GetByCustomerAsync(
                        customerId));
        }

        [HttpPost]
        public async Task<IActionResult>
            Create(CreateBookingDto dto)
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
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                // Important for Angular seat/parking refresh
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("{bookingId:int}/seats")]
        public async Task<IActionResult>
            AddSeats(
                int bookingId,
                AttachSeatsDto dto)
        {
            try
            {
                await _service
                    .AddSeatsAsync(
                        bookingId,
                        dto);

                return Ok(new
                {
                    message =
                        "Seats added successfully."
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

        [HttpPost(
            "{bookingId:int}/parking")]
        public async Task<IActionResult>
            ReserveParking(
                int bookingId,
                ReserveParkingDto dto)
        {
            try
            {
                await _service
                    .ReserveParkingAsync(
                        bookingId,
                        dto);

                return Ok(new
                {
                    message =
                        "Parking reserved successfully."
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

        [HttpDelete(
            "{bookingId:int}/parking")]
        public async Task<IActionResult>
            RemoveParking(int bookingId)
        {
            try
            {
                await _service
                    .RemoveParkingAsync(
                        bookingId);

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

        [HttpDelete("{bookingId:int}")]
        public async Task<IActionResult>
            Cancel(int bookingId)
        {
            try
            {
                await _service
                    .CancelAsync(bookingId);

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
