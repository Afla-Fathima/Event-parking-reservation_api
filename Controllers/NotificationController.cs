using EventParkingReservation.Security;
using EventParkingReservation.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservation.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationController :
        ControllerBase
    {
        private readonly INotificationService
            _service;

        public NotificationController(
            INotificationService service)
        {
            _service = service;
        }

        [HttpGet("my")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult>
            GetMyNotifications()
        {
            return Ok(
                await _service
                    .GetByCustomerAsync(
                        User.GetCustomerId()));
        }

        [HttpGet("my/unread-count")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult>
            GetMyUnreadCount()
        {
            return Ok(new
            {
                count =
                    await _service
                        .GetUnreadCountAsync(
                            User.GetCustomerId())
            });
        }

        // BRD-compatible route.
        [HttpGet(
            "customer/{customerId:int}")]
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
            "customer/{customerId:int}/unread-count")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult>
            GetUnreadCount(
                int customerId)
        {
            if (User.IsInRole("Customer") &&
                User.GetCustomerId() !=
                    customerId)
            {
                return Forbid();
            }

            return Ok(new
            {
                count =
                    await _service
                        .GetUnreadCountAsync(
                            customerId)
            });
        }

        [HttpPut(
            "{notificationId:int}/read")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult>
            MarkAsRead(
                int notificationId)
        {
            try
            {
                await _service
                    .MarkAsReadAsync(
                        notificationId,
                        User.GetCustomerId());

                return NoContent();
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
    }
}