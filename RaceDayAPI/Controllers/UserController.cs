using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;
using RaceDayAPI.Services;
using System.Security.Claims;

namespace RaceDayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly RaceDayDbContext _context;
        private readonly BlobService _blobService;

        public UserController(RaceDayDbContext context, BlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        // GET: /api/user/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserID == userId);

            if (user == null) return NotFound("User not found.");

            if (user.Role == "Organiser")
            {
                var organiser = await _context.Organisers.FirstOrDefaultAsync(o => o.UserID == userId);
                return Ok(new
                {
                    user.UserID,
                    user.Email,
                    user.Role,
                    user.FullName,
                    user.PhoneNumber,
                    user.ProfilePictureUrl,
                    organiser?.OrganisationName,
                    organiser?.BusinessPhone
                });
            }
            else
            {
                var participant = await _context.Participants.FirstOrDefaultAsync(p => p.UserID == userId);
                return Ok(new
                {
                    user.UserID,
                    user.Email,
                    user.Role,
                    user.FullName,
                    user.PhoneNumber,
                    user.ProfilePictureUrl,
                    participant?.DateOfBirth,
                    participant?.EmergencyContactName,
                    participant?.EmergencyContactPhone
                });
            }
        }

        // PUT: /api/user/profile
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserID == userId);

            if (user == null) return NotFound("User not found.");

            user.FullName = dto.FullName ?? user.FullName;
            user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;

            if (user.Role == "Organiser")
            {
                var organiser = await _context.Organisers.FirstOrDefaultAsync(o => o.UserID == userId);
                if (organiser != null)
                {
                    organiser.OrganisationName = dto.OrganisationName ?? organiser.OrganisationName;
                    organiser.BusinessPhone = dto.BusinessPhone ?? organiser.BusinessPhone;
                }
            }
            else
            {
                var participant = await _context.Participants.FirstOrDefaultAsync(p => p.UserID == userId);
                if (participant != null)
                {
                    participant.EmergencyContactName = dto.EmergencyContactName ?? participant.EmergencyContactName;
                    participant.EmergencyContactPhone = dto.EmergencyContactPhone ?? participant.EmergencyContactPhone;
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: /api/user/upload-profile-picture
        [HttpPost("upload-profile-picture")]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserID == userId);
            if (user == null) return NotFound();

            var imageUrl = await _blobService.UploadFileAsync(file);

            user.ProfilePictureUrl = imageUrl;
            await _context.SaveChangesAsync();

            return Ok(new { url = imageUrl });
        }
    }

    public class UpdateProfileDto
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? OrganisationName { get; set; }
        public string? BusinessPhone { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
    }
}