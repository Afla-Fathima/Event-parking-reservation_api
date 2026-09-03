using EventParkingReservation.DTOs.Payment;
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

        [HttpGet("bookings/{bookingId:int}/payment")]
        public async Task<IActionResult> GetPaymentStatus(
            int bookingId)
        {
            try
            {
                var result =
                    await _service.GetStatusAsync(
                        bookingId);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("bookings/{bookingId:int}/payment")]
        public async Task<IActionResult> Pay(
            int bookingId,
            CreatePaymentDto dto)
        {
            try
            {
                var result =
                    await _service.PayAsync(
                        bookingId,
                        dto);

                return StatusCode(
                    StatusCodes.Status201Created,
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

        [HttpGet("payments/customer/{customerId:int}")]
        public async Task<IActionResult> GetByCustomer(
            int customerId)
        {
            var result =
                await _service.GetByCustomerAsync(
                    customerId);

            return Ok(result);
        }

        [HttpGet("payments/{paymentId:int}/receipt")]
        public async Task<IActionResult> GetReceipt(
            int paymentId)
        {
            try
            {
                var result =
                    await _service.GetReceiptAsync(
                        paymentId);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
