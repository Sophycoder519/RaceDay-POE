using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Controllers;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Models;
using RaceDayAPI.Services;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace RaceDayAPI.Tests
{
    public class AuthControllerTests
    {
        private RaceDayDbContext GetInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new RaceDayDbContext(options);
        }

        private JwtService GetJwtService()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "Jwt:Key", "ThisIsMySecretKeyForRaceDayApplication2026!" },
                    { "Jwt:Issuer", "RaceDayAPI" },
                    { "Jwt:Audience", "RaceDayClient" }
                })
                .Build();
            return new JwtService(config);
        }

        [Fact]
        public async Task Register_WithValidOrganiser_ReturnsOkWithToken()
        {
            var db = GetInMemoryDb();
            var controller = new AuthController(db, GetJwtService());
            var dto = new RegisterDto
            {
                Email = "test@test.com",
                Password = "password123",
                Role = "Organiser",
                FullName = "Test User",
                OrganisationName = "Test Org"
            };

            var result = await controller.Register(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthResponseDto>(okResult.Value);
            Assert.NotNull(response.Token);
            Assert.Equal("Organiser", response.Role);
        }

        [Fact]
        public async Task Register_WithValidParticipant_ReturnsOkWithToken()
        {
            var db = GetInMemoryDb();
            var controller = new AuthController(db, GetJwtService());
            var dto = new RegisterDto
            {
                Email = "participant@test.com",
                Password = "password123",
                Role = "Participant",
                FullName = "Test Participant",
                DateOfBirth = new DateTime(1995, 5, 15)
            };

            var result = await controller.Register(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthResponseDto>(okResult.Value);
            Assert.NotNull(response.Token);
        }

        [Fact]
        public async Task Register_WithInvalidRole_ReturnsBadRequest()
        {
            var db = GetInMemoryDb();
            var controller = new AuthController(db, GetJwtService());
            var dto = new RegisterDto
            {
                Email = "test@test.com",
                Password = "password123",
                Role = "InvalidRole",
                FullName = "Test"
            };

            var result = await controller.Register(dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_WithDuplicateEmail_ReturnsBadRequest()
        {
            var db = GetInMemoryDb();
            var controller = new AuthController(db, GetJwtService());

            await controller.Register(new RegisterDto
            {
                Email = "dupe@test.com",
                Password = "password123",
                Role = "Organiser",
                FullName = "First"
            });

            var result = await controller.Register(new RegisterDto
            {
                Email = "dupe@test.com",
                Password = "password123",
                Role = "Participant",
                FullName = "Second"
            });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkWithToken()
        {
            var db = GetInMemoryDb();
            var controller = new AuthController(db, GetJwtService());

            await controller.Register(new RegisterDto
            {
                Email = "login@test.com",
                Password = "password123",
                Role = "Participant",
                FullName = "Test"
            });

            var result = await controller.Login(new LoginDto
            {
                Email = "login@test.com",
                Password = "password123"
            });

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthResponseDto>(okResult.Value);
            Assert.NotNull(response.Token);
        }

        [Fact]
        public async Task Login_WithWrongPassword_ReturnsUnauthorized()
        {
            var db = GetInMemoryDb();
            var controller = new AuthController(db, GetJwtService());

            await controller.Register(new RegisterDto
            {
                Email = "wrong@test.com",
                Password = "password123",
                Role = "Participant",
                FullName = "Test"
            });

            var result = await controller.Login(new LoginDto
            {
                Email = "wrong@test.com",
                Password = "wrongpassword"
            });

            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}