using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Models;
using RaceDayMVC.Services;

namespace RaceDayMVC.Controllers
{
    public class ParticipantController : Controller
    {
        private readonly ApiService _apiService;

        public ParticipantController(ApiService apiService)
        {
            _apiService = apiService;
        }

        private bool IsParticipant() => HttpContext.Session.GetString("UserRole") == "Participant";

        public async Task<IActionResult> Index()
        {
            var events = await _apiService.GetAsync<List<EventModel>>("api/events");
            return View(events ?? new List<EventModel>());
        }

        public async Task<IActionResult> Details(int id)
        {
            if (!IsParticipant()) return RedirectToAction("Login", "Account");
            var ev = await _apiService.GetAsync<EventModel>($"api/events/{id}");
            return View(ev);
        }

        [HttpPost]
        public async Task<IActionResult> Enrol(int eventId, int categoryId)
        {
            if (!IsParticipant()) return RedirectToAction("Login", "Account");
            var payload = new { EventID = eventId, CategoryID = categoryId };
            var response = await _apiService.PostAsync<object, dynamic>("api/enrolments", payload);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> MyEnrolments()
        {
            if (!IsParticipant()) return RedirectToAction("Login", "Account");
            var enrolments = await _apiService.GetAsync<List<dynamic>>("api/enrolments/me");
            return View(enrolments ?? new List<dynamic>());
        }

        public async Task<IActionResult> MyResults()
        {
            if (!IsParticipant()) return RedirectToAction("Login", "Account");
            var results = await _apiService.GetAsync<List<dynamic>>("api/results/me");
            return View(results ?? new List<dynamic>());
        }

        public async Task<IActionResult> Profile()
        {
            if (HttpContext.Session.GetString("JwtToken") == null) return RedirectToAction("Login", "Account");
            var profile = await _apiService.GetAsync<dynamic>("api/user/profile");
            return View(profile);
        }
    }
}
