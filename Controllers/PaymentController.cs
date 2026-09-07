using EventParkingReservation.DTOs.Payment;
using EventParkingReservation.Security;
using EventParkingReservation.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservation.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _service;

        public PaymentController(
            IPaymentService service)
        {
            _service = service;
        }

        [HttpGet(
            "bookings/{bookingId:int}/payment")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult>
            GetPaymentStatus(
                int bookingId)
        {
            try
            {
                return Ok(
                    await _service.GetStatusAsync(
                        bookingId,
                        GetCustomerScope()));
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
        }

        [HttpPost(
            "bookings/{bookingId:int}/payment")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> Pay(
            int bookingId,
            CreatePaymentDto dto)
        {
            try
            {
                var result =
                    await _service.PayAsync(
                        bookingId,
                        GetCustomerScope(),
                        dto);

                return StatusCode(
                    StatusCodes
                        .Status201Created,
                    result);
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

        [HttpGet("payments/my")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult>
            GetMyPayments()
        {
            return Ok(
                await _service
                    .GetByCustomerAsync(
                        User.GetCustomerId()));
        }

        [HttpGet(
            "payments/customer/{customerId:int}")]
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

        [HttpGet(
            "payments/{paymentId:int}/receipt")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult>
            GetReceipt(
                int paymentId)
        {
            try
            {
                return Ok(
                    await _service
                        .GetReceiptAsync(
                            paymentId,
                            GetCustomerScope()));
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
        }

        private int? GetCustomerScope()
        {
            return User.IsInRole("Admin")
                ? null
                : User.GetCustomerId();
        }
    }
}