using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Controllers;
using RaceDayAPI.Data;
using RaceDayAPI.Models;
using System.Security.Claims;
using Xunit;

namespace RaceDayAPI.Tests
{
    public class EventsControllerTests
    {
        private RaceDayDbContext GetInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new RaceDayDbContext(options);
        }

        private void SetupUserContext(ControllerBase controller, int userId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        [Fact]
        public async Task GetAllEvents_ReturnsOk()
        {
            var db = GetInMemoryDb();
            var controller = new EventsController(db);

            var result = await controller.GetAllEvents();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetEvent_WithInvalidId_ReturnsNotFound()
        {
            var db = GetInMemoryDb();
            var controller = new EventsController(db);

            var result = await controller.GetEvent(999);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task CreateEvent_WithOrganiserRole_ReturnsCreated()
        {
            var db = GetInMemoryDb();
            var user = new User { Email = "org@test.com", PasswordHash = "x", Role = "Organiser", FullName = "Org" };
            db.Users.Add(user);
            await db.SaveChangesAsync();
            db.Organisers.Add(new Organiser { UserID = user.UserID, OrganisationName = "Test" });
            await db.SaveChangesAsync();

            var controller = new EventsController(db);
            SetupUserContext(controller, user.UserID, "Organiser");

            var result = await controller.CreateEvent(new CreateEventDto
            {
                Title = "New Event",
                EventDate = DateTime.Now.AddDays(30),
                Location = "Durban",
                MaxParticipants = 100
            });

            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task CreateEvent_WithNonOrganiser_ReturnsForbid()
        {
            var db = GetInMemoryDb();
            var user = new User { Email = "part@test.com", PasswordHash = "x", Role = "Participant", FullName = "Part" };
            db.Users.Add(user);
            await db.SaveChangesAsync();

            var controller = new EventsController(db);
            SetupUserContext(controller, user.UserID, "Participant");

            var result = await controller.CreateEvent(new CreateEventDto
            {
                Title = "Should Fail",
                EventDate = DateTime.Now.AddDays(30),
                Location = "Nowhere"
            });

            Assert.IsType<ForbidResult>(result);
        }
    }
}