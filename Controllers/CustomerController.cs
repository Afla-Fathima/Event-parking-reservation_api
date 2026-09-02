using Microsoft.AspNetCore.Mvc;

namespace Event_Parking_Reservation.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
