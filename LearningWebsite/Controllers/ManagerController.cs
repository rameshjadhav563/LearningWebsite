using LearningWebsite.Data;
using LearningWebsite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LearningWebsite.Controllers
{
    [Authorize(Policy = "ManagerOnly")]
    public class ManagerController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly ILogger<ManagerController> _logger;

        public ManagerController(AppDbContext context, IPasswordHasher<ApplicationUser> passwordHasher, ILogger<ManagerController> logger)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Manager dashboard opened by {User}", User.Identity?.Name);
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var manager = _context.Users.FirstOrDefault(u => u.UserName == userName);

            if (manager == null)
            {
                return NotFound();
            }

            // Get all team members assigned to this manager
            var teamMembers = _context.Users
                .Where(u => u.ManagerId == manager.Id && u.Role == "Employee")
                .OrderBy(u => u.UserName)
                .ToList();

            ViewBag.Manager = manager;
            ViewBag.TeamMembers = teamMembers;
            ViewBag.TeamMemberCount = teamMembers.Count;

            return View();
        }

        [HttpGet]
        public IActionResult TeamMemberDetail(int id)
        {
            _logger.LogInformation("Manager viewing team member {MemberId}", id);
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var manager = _context.Users.FirstOrDefault(u => u.UserName == userName);

            if (manager == null)
            {
                return NotFound();
            }

            var teamMember = _context.Users
                .Include(u => u.Assignments)
                    .ThenInclude(a => a.Learning)
                .FirstOrDefault(u => u.Id == id && u.ManagerId == manager.Id);

            if (teamMember == null)
            {
                return Forbid();
            }

            ViewBag.TeamMember = teamMember;
            ViewBag.Assignments = teamMember.Assignments;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignLearning(int memberId, int learningId)
        {
            _logger.LogInformation("Manager assigning learning {LearningId} to member {MemberId}", learningId, memberId);
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var manager = _context.Users.FirstOrDefault(u => u.UserName == userName);

            if (manager == null)
            {
                return NotFound();
            }

            var teamMember = _context.Users.FirstOrDefault(u => u.Id == memberId && u.ManagerId == manager.Id);
            if (teamMember == null)
            {
                return Forbid();
            }

            var learning = _context.Learnings.FirstOrDefault(l => l.Id == learningId);
            if (learning == null)
            {
                return NotFound();
            }

            // Check if already assigned
            var existingAssignment = _context.LearningAssignments
                .FirstOrDefault(a => a.UserId == memberId && a.LearningId == learningId);

            if (existingAssignment == null)
            {
                var assignment = new LearningAssignment
                {
                    UserId = memberId,
                    LearningId = learningId,
                    AssignedDate = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(30),
                    Status = "NotStarted",
                    ProgressPercentage = 0
                };

                _context.LearningAssignments.Add(assignment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("TeamMemberDetail", new { id = memberId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignMultipleLearnings(int memberId, List<int> learningIds, DateTime? dueDate)
        {
            _logger.LogInformation("Manager assigning multiple learnings to member {MemberId}", memberId);
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var manager = _context.Users.FirstOrDefault(u => u.UserName == userName);

            if (manager == null)
            {
                return NotFound();
            }

            var teamMember = _context.Users.FirstOrDefault(u => u.Id == memberId && u.ManagerId == manager.Id);
            if (teamMember == null)
            {
                return Forbid();
            }

            if (learningIds == null || !learningIds.Any())
            {
                TempData["ErrorMessage"] = "Please select at least one learning to assign.";
                return RedirectToAction("TeamMemberDetail", new { id = memberId });
            }

            var assignmentDueDate = dueDate ?? DateTime.Now.AddDays(30);
            int assignedCount = 0;
            int skippedCount = 0;

            foreach (var learningId in learningIds)
            {
                var learning = _context.Learnings.FirstOrDefault(l => l.Id == learningId);
                if (learning == null) continue;

                // Check if already assigned
                var existingAssignment = _context.LearningAssignments
                    .FirstOrDefault(a => a.UserId == memberId && a.LearningId == learningId);

                if (existingAssignment == null)
                {
                    var assignment = new LearningAssignment
                    {
                        UserId = memberId,
                        LearningId = learningId,
                        AssignedDate = DateTime.Now,
                        DueDate = assignmentDueDate,
                        Status = "NotStarted",
                        ProgressPercentage = 0
                    };

                    _context.LearningAssignments.Add(assignment);
                    assignedCount++;
                }
                else
                {
                    skippedCount++;
                }
            }

            if (assignedCount > 0)
            {
                await _context.SaveChangesAsync();

                var message = $"Successfully assigned {assignedCount} learning(s) to {teamMember.UserName}.";
                if (skippedCount > 0)
                {
                    message += $" {skippedCount} learning(s) were already assigned.";
                }

                TempData["SuccessMessage"] = message;
                _logger.LogInformation("Manager {Manager} assigned {Count} learnings to member {Member}", 
                    manager.UserName, assignedCount, teamMember.UserName);
            }
            else
            {
                TempData["InfoMessage"] = "All selected learnings were already assigned to this user.";
            }

            return RedirectToAction("TeamMemberDetail", new { id = memberId });
        }

        // View Profile
        [HttpGet]
        public IActionResult MyProfile()
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var manager = _context.Users
                .Include(u => u.Manager)
                .Include(u => u.TeamMembers)
                .FirstOrDefault(u => u.UserName == userName);

            if (manager == null)
            {
                return NotFound();
            }

            _logger.LogInformation("Manager {User} viewing profile", userName);
            return View(manager);
        }

        // Edit Profile - GET
        [HttpGet]
        public IActionResult EditProfile()
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var manager = _context.Users.FirstOrDefault(u => u.UserName == userName);

            if (manager == null)
            {
                return NotFound();
            }

            _logger.LogInformation("Manager {User} editing profile", userName);
            return View(manager);
        }

        // Edit Profile - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(ApplicationUser model, string? newPassword, string? confirmPassword)
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var manager = _context.Users.FirstOrDefault(u => u.UserName == userName);

            if (manager == null)
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

                manager.PasswordHash = _passwordHasher.HashPassword(manager, newPassword);
            }

            // Update allowed fields
            manager.FullName = model.FullName;
            manager.Email = model.Email;

            _context.Update(manager);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Manager {User} updated profile", userName);
            TempData["SuccessMessage"] = "Profile updated successfully!";

            return RedirectToAction(nameof(MyProfile));
        }
    }
}