using Microsoft.AspNetCore.Mvc;

namespace RaceDayMVC.Controllers
{
    public class OrganiserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
