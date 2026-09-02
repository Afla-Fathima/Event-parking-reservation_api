using Microsoft.AspNetCore.Mvc;

namespace Event_Parking_Reservation.Controllers
{
    public class EventController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
