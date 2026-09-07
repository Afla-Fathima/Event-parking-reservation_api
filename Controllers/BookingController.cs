using EventParkingReservation.DTOs.Booking;
using EventParkingReservation.Security;
using EventParkingReservation.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservation.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _service;

        public BookingController(
            IBookingService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create(
            CreateBookingDto dto)
        {
            try
            {
                dto.CustomerId =
                    User.GetCustomerId();

                var result =
                    await _service.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new
                    {
                        id =
                            result.BookingId
                    },
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

        [HttpPost("{bookingId:int}/seats")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddSeats(
            int bookingId,
            AttachSeatsDto dto)
        {
            try
            {
                return Ok(
                    await _service.AddSeatsAsync(
                        bookingId,
                        User.GetCustomerId(),
                        dto));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
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

        [HttpPost("{bookingId:int}/parking")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult>
            ReserveParking(
                int bookingId,
                ReserveParkingDto dto)
        {
            try
            {
                return Ok(
                    await _service
                        .ReserveParkingAsync(
                            bookingId,
                            User.GetCustomerId(),
                            dto));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
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

        [HttpDelete("{bookingId:int}/parking")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult>
            RemoveParking(
                int bookingId)
        {
            try
            {
                return Ok(
                    await _service
                        .RemoveParkingAsync(
                            bookingId,
                            User.GetCustomerId()));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
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

        [HttpGet("my")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult>
            GetMyBookings()
        {
            return Ok(
                await _service
                    .GetByCustomerAsync(
                        User.GetCustomerId()));
        }

        [HttpGet("customer/{customerId:int}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult>
            GetByCustomer(
                int customerId)
        {
            if (User.IsInRole("Customer") &&
                User.GetCustomerId() !=
                    customerId)
            {
                return Forbid();
            }

            return Ok(
                await _service
                    .GetByCustomerAsync(
                        customerId));
        }

        [HttpGet("my/{id:int}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult>
            GetMyBooking(
                int id)
        {
            var booking =
                await _service.GetByIdAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            if (booking.CustomerId !=
                User.GetCustomerId())
            {
                return Forbid();
            }

            return Ok(booking);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetById(
            int id)
        {
            var booking =
                await _service.GetByIdAsync(id);

            if (booking == null)
            {
                return NotFound(new
                {
                    message =
                        "Booking not found."
                });
            }

            if (User.IsInRole("Customer") &&
                booking.CustomerId !=
                    User.GetCustomerId())
            {
                return Forbid();
            }

            return Ok(booking);
        }

        [HttpDelete("my/{id:int}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult>
            CancelMyBooking(
                int id)
        {
            return await CancelOwnedBooking(
                id);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Cancel(
            int id)
        {
            return await CancelOwnedBooking(
                id);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult>
            GetByEvent(
                [FromQuery] int eventId)
        {
            if (eventId <= 0)
            {
                return BadRequest(new
                {
                    message =
                        "Valid eventId is required."
                });
            }

            return Ok(
                await _service
                    .GetByEventAsync(
                        eventId));
        }

        private async Task<IActionResult>
            CancelOwnedBooking(
                int id)
        {
            try
            {
                var booking =
                    await _service
                        .GetByIdAsync(id);

                if (booking == null)
                {
                    return NotFound(new
                    {
                        message =
                            "Booking not found."
                    });
                }

                if (booking.CustomerId !=
                    User.GetCustomerId())
                {
                    return Forbid();
                }

                await _service.CancelAsync(id);

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