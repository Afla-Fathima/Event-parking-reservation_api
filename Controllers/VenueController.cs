using Microsoft.AspNetCore.Mvc;

namespace Event_Parking_Reservation.Controllers
{
    public class VenueController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
