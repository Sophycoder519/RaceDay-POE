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
    public class EnrolmentsControllerTests
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
        public async Task CreateEnrolment_WithParticipantRole_Succeeds()
        {
            var db = GetInMemoryDb();

            var orgUser = new User { Email = "org@test.com", PasswordHash = "x", Role = "Organiser", FullName = "Org" };
            var partUser = new User { Email = "part@test.com", PasswordHash = "x", Role = "Participant", FullName = "Part" };
            db.Users.AddRange(orgUser, partUser);
            await db.SaveChangesAsync();

            var organiser = new Organiser { UserID = orgUser.UserID, OrganisationName = "Test Org" };
            var participant = new Participant { UserID = partUser.UserID, DateOfBirth = DateTime.Now.AddYears(-25) };
            db.Organisers.Add(organiser);
            db.Participants.Add(participant);
            await db.SaveChangesAsync();

            var ev = new Event { OrganiserID = organiser.OrganiserID, Title = "Test Event", Location = "X", EventDate = DateTime.Now.AddDays(30) };
            db.Events.Add(ev);
            await db.SaveChangesAsync();

            var cat = new Category { EventID = ev.EventID, Name = "5km", Distance = 5, EntryFee = 50, StartTime = TimeSpan.FromHours(6) };
            db.Categories.Add(cat);
            await db.SaveChangesAsync();

            var controller = new EnrolmentsController(db);
            SetupUserContext(controller, partUser.UserID, "Participant");

            var result = await controller.CreateEnrolment(new EnrolmentDto
            {
                EventID = ev.EventID,
                CategoryID = cat.CategoryID
            });

            Assert.IsType<OkObjectResult>(result);

            var enrolment = await db.Enrolments.FirstOrDefaultAsync();
            Assert.NotNull(enrolment);
            Assert.Equal(participant.ParticipantID, enrolment.ParticipantID);
        }

        [Fact]
        public async Task CreateEnrolment_WithOrganiserRole_ReturnsForbid()
        {
            var db = GetInMemoryDb();

            var orgUser = new User { Email = "org@test.com", PasswordHash = "x", Role = "Organiser", FullName = "Org" };
            db.Users.Add(orgUser);
            await db.SaveChangesAsync();
            db.Organisers.Add(new Organiser { UserID = orgUser.UserID, OrganisationName = "Test" });
            await db.SaveChangesAsync();

            var controller = new EnrolmentsController(db);
            SetupUserContext(controller, orgUser.UserID, "Organiser");

            var result = await controller.CreateEnrolment(new EnrolmentDto { EventID = 1, CategoryID = 1 });

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task CreateEnrolment_WhenAlreadyEnrolled_ReturnsConflict()
        {
            var db = GetInMemoryDb();

            var orgUser = new User { Email = "org@test.com", PasswordHash = "x", Role = "Organiser", FullName = "Org" };
            var partUser = new User { Email = "part@test.com", PasswordHash = "x", Role = "Participant", FullName = "Part" };
            db.Users.AddRange(orgUser, partUser);
            await db.SaveChangesAsync();

            var organiser = new Organiser { UserID = orgUser.UserID, OrganisationName = "Test" };
            var participant = new Participant { UserID = partUser.UserID, DateOfBirth = DateTime.Now.AddYears(-25) };
            db.Organisers.Add(organiser);
            db.Participants.Add(participant);
            await db.SaveChangesAsync();

            var ev = new Event { OrganiserID = organiser.OrganiserID, Title = "Test", Location = "X", EventDate = DateTime.Now.AddDays(30) };
            db.Events.Add(ev);
            await db.SaveChangesAsync();

            var cat = new Category { EventID = ev.EventID, Name = "5km", Distance = 5, EntryFee = 50, StartTime = TimeSpan.FromHours(6) };
            db.Categories.Add(cat);
            await db.SaveChangesAsync();

            var controller = new EnrolmentsController(db);
            SetupUserContext(controller, partUser.UserID, "Participant");

            await controller.CreateEnrolment(new EnrolmentDto { EventID = ev.EventID, CategoryID = cat.CategoryID });

            var result = await controller.CreateEnrolment(new EnrolmentDto { EventID = ev.EventID, CategoryID = cat.CategoryID });

            Assert.IsType<ConflictObjectResult>(result);
        }
    }
}