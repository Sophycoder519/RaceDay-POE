using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Models;
using RaceDayMVC.Services;

namespace RaceDayMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiService _apiService;

        public AccountController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("JwtToken") != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var response = await _apiService.PostAsync<LoginViewModel, AuthResponse>("api/auth/login", model);

            if (response != null && !string.IsNullOrEmpty(response.Token))
            {
                // Save token and role in Session
                HttpContext.Session.SetString("JwtToken", response.Token);
                HttpContext.Session.SetString("UserRole", response.Role);
                HttpContext.Session.SetString("UserName", response.FullName);

                // Redirect based on role
                if (response.Role == "Organiser")
                    return RedirectToAction("Dashboard", "Organiser");

                return RedirectToAction("Index", "Participant");
            }

            ViewBag.Error = "Invalid email or password.";
            return View(model);
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var response = await _apiService.PostAsync<RegisterViewModel, AuthResponse>("api/auth/register", model);

            if (response != null && !string.IsNullOrEmpty(response.Token))
            {
                // Auto login after registration
                HttpContext.Session.SetString("JwtToken", response.Token);
                HttpContext.Session.SetString("UserRole", response.Role);
                HttpContext.Session.SetString("UserName", response.FullName);

                if (response.Role == "Organiser")
                    return RedirectToAction("Dashboard", "Organiser");

                return RedirectToAction("Index", "Participant");
            }

            ViewBag.Error = "Registration failed. Email might already exist.";
            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}