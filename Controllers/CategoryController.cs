using Microsoft.AspNetCore.Mvc;

namespace Event_Parking_Reservation.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
