using LearningWebsite.Data;
using LearningWebsite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LearningWebsite.Controllers
{
    [Authorize(Policy = "EmployeeOnly")]
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(AppDbContext context, IPasswordHasher<ApplicationUser> passwordHasher, ILogger<EmployeeController> logger)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Employee dashboard opened by {User}", User.Identity?.Name);
            return View();
        }

        // View Profile
        [HttpGet]
        public IActionResult MyProfile()
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var employee = _context.Users
                .Include(u => u.Manager)
                .Include(u => u.Assignments)
                    .ThenInclude(a => a.Learning)
                .FirstOrDefault(u => u.UserName == userName);

            if (employee == null)
            {
                return NotFound();
            }

            _logger.LogInformation("Employee {User} viewing profile", userName);
            return View(employee);
        }

        // Edit Profile - GET
        [HttpGet]
        public IActionResult EditProfile()
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var employee = _context.Users.FirstOrDefault(u => u.UserName == userName);

            if (employee == null)
            {
                return NotFound();
            }

            _logger.LogInformation("Employee {User} editing profile", userName);
            return View(employee);
        }

        // Edit Profile - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(ApplicationUser model, string? newPassword, string? confirmPassword)
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var employee = _context.Users.FirstOrDefault(u => u.UserName == userName);

            if (employee == null)
            {
                return NotFound();
            }

            // Validate password if provided
            if (!string.IsNullOrEmpty(newPassword))
            {
                if (newPassword != confirmPassword)
                {
                    ModelState.AddModelError("", "Passwords do not match");
                    return View(model);
                }

                if (newPassword.Length < 6)
                {
                    ModelState.AddModelError("", "Password must be at least 6 characters long");
                    return View(model);
                }

                employee.PasswordHash = _passwordHasher.HashPassword(employee, newPassword);
            }

            // Update allowed fields
            employee.FullName = model.FullName;
            employee.Email = model.Email;

            _context.Update(employee);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Employee {User} updated profile", userName);
            TempData["SuccessMessage"] = "Profile updated successfully!";

            return RedirectToAction(nameof(MyProfile));
        }
    }
}