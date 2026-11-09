using Microsoft.AspNetCore.Mvc;
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
            var events = await _apiService.GetAsync<List<dynamic>>("api/events");
            return View(events ?? new List<dynamic>());
        }

        public async Task<IActionResult> Details(int id)
        {
            if (!IsParticipant()) return RedirectToAction("Login", "Account");
            var ev = await _apiService.GetAsync<dynamic>($"api/events/{id}");
            return View(ev);
        }

        [HttpPost]
        public async Task<IActionResult> Enrol(int eventId, int categoryId)
        {
            if (!IsParticipant()) return RedirectToAction("Login", "Account");
            var payload = new { EventID = eventId, CategoryID = categoryId };
            var response = await _apiService.PostAsync<object, dynamic>("api/enrolments", payload);
            return RedirectToAction("MyEnrolments");
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

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(string fullName, string phoneNumber)
        {
            if (!IsParticipant()) return RedirectToAction("Login", "Account");
            var payload = new { FullName = fullName, PhoneNumber = phoneNumber };
            await _apiService.PutAsync("api/user/profile", payload);
            return RedirectToAction("Profile");
        }

        [HttpPost]
        public async Task<IActionResult> UploadProfilePicture(IFormFile profilePic)
        {
            if (profilePic != null && profilePic.Length > 0)
            {
                await _apiService.UploadFileAsync("api/user/upload-profile-picture", profilePic);
            }
            return RedirectToAction("Profile");
        }
    }
}