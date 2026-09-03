using EventParkingReservation.Services.Implementations;
using EventParkingReservation.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        [HttpGet(
            "customer/{customerId:int}")]
        public async Task<IActionResult>
            GetByCustomer(int customerId)
        {
            return Ok(
                await _service
                    .GetByCustomerAsync(
                        customerId));
        }

        [HttpGet(
            "customer/{customerId:int}/unread-count")]
        public async Task<IActionResult>
            GetUnreadCount(int customerId)
        {
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
        public async Task<IActionResult>
            MarkAsRead(
                int notificationId,
                int customerId)
        {
            try
            {
                await _service
                    .MarkAsReadAsync(
                        notificationId,
                        customerId);

                return Ok(new
                {
                    message =
                        "Notification marked as read."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
        }
    }
}
