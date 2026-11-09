using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Services;

namespace RaceDayMVC.Controllers
{
    public class OrganiserController : Controller
    {
        private readonly ApiService _apiService;

        public OrganiserController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // Helper to check if user is Organiser
        private bool IsOrganiser()
        {
            return HttpContext.Session.GetString("UserRole") == "Organiser";
        }

        public async Task<IActionResult> Dashboard()
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Account");
            var events = await _apiService.GetAsync<List<dynamic>>("api/events");
            ViewBag.EventCount = events?.Count ?? 0;
            return View(events ?? new List<dynamic>()); // Pass the list to the view
        }

        public async Task<IActionResult> Events()
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Account");

            var events = await _apiService.GetAsync<List<dynamic>>("api/events");
            return View(events ?? new List<dynamic>());
        }

        [HttpGet]
        public IActionResult CreateEvent()
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent(object eventDto)
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Account");

            // We use dynamic object to pass the form data to the API
            var response = await _apiService.PostAsync<object, dynamic>("api/events", eventDto);

            if (response != null)
            {
                return RedirectToAction("Events");
            }

            ViewBag.Error = "Failed to create event. Check your data.";
            return View(eventDto);
        }
    }
}