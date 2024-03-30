using Microsoft.AspNetCore.Mvc;

namespace E_BookingFutsal.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
