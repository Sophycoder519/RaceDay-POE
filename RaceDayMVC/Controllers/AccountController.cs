using Microsoft.AspNetCore.Mvc;

namespace RaceDayMVC.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
