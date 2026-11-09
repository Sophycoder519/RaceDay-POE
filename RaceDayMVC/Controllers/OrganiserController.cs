using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Models;
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

        private bool IsOrganiser() => HttpContext.Session.GetString("UserRole") == "Organiser";

        public async Task<IActionResult> Dashboard()
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Account");
            var events = await _apiService.GetAsync<List<EventModel>>("api/events");
            ViewBag.EventCount = events?.Count ?? 0;
            return View(events ?? new List<EventModel>());
        }

        public async Task<IActionResult> Events()
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Account");
            var events = await _apiService.GetAsync<List<EventModel>>("api/events");
            return View(events ?? new List<EventModel>());
        }

        [HttpGet]
        public IActionResult CreateEvent()
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent(EventModel eventDto)
        {
            if (!IsOrganiser()) return RedirectToAction("Login", "Account");

            var payload = new
            {
                Title = eventDto.Title,
                Description = eventDto.Description,
                EventDate = eventDto.EventDate,
                Location = eventDto.Location,
                EventType = eventDto.EventType,
                MaxParticipants = 500
            };

            var response = await _apiService.PostAsync<object, dynamic>("api/events", payload);
            if (response != null) return RedirectToAction("Events");

            ViewBag.Error = "Failed to create event.";
            return View(eventDto);
        }
    }
}