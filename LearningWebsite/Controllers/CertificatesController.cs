using LearningWebsite.Data;
using LearningWebsite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LearningWebsite.Controllers
{
    [Authorize(Policy = "EmployeeOnly")]
    public class CertificatesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CertificatesController> _logger;

        public CertificatesController(AppDbContext context, ILogger<CertificatesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: My Certificates
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                _logger.LogInformation("User {UserId} ({UserName}) viewing certificates", 
                    userId, User.Identity?.Name);

                var certificates = await _context.Certificates
                    .Include(c => c.Learning)
                    .Where(c => c.UserId == userId)
                    .OrderByDescending(c => c.IssuedDate)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} certificates for User {UserId}", 
                    certificates.Count, userId);

                return View(certificates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading certificates");
                return StatusCode(500, "An error occurred while loading certificates.");
            }
        }

        // GET: View Certificate Details
        public async Task<IActionResult> View(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var certificate = await _context.Certificates
                    .Include(c => c.Learning)
                    .Include(c => c.AssessmentResult)
                    .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

                if (certificate == null)
                {
                    return NotFound("Certificate not found.");
                }

                return View(certificate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading certificate {CertificateId}", id);
                return StatusCode(500, "An error occurred while loading the certificate.");
            }
        }
    }
}
