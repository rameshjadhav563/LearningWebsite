using LearningWebsite.Data;
using LearningWebsite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LearningWebsite.Controllers
{
    [Authorize(Policy = "HROnly")]
    public class HRController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly ILogger<HRController> _logger;

        public HRController(AppDbContext context, IPasswordHasher<ApplicationUser> passwordHasher, ILogger<HRController> logger)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        // Dashboard
        public IActionResult Index()
        {
            _logger.LogInformation("HR dashboard opened by {User}", User.Identity?.Name);

            var users = _context.Users.Include(u => u.Manager).ToList();
            var employees = users.Where(u => u.Role == "Employee").ToList();
            var managers = users.Where(u => u.Role == "Manager").ToList();
            var hrUsers = users.Where(u => u.Role == "HR").ToList();

            // Get all assignments with related data
            var allAssignments = _context.LearningAssignments
                .Include(a => a.User)
                .Include(a => a.Learning)
                .ToList();

            // Calculate metrics
            var totalAssignments = allAssignments.Count;
            var completedAssignments = allAssignments.Count(a => a.Status == "Completed");
            var inProgressAssignments = allAssignments.Count(a => a.Status == "InProgress");
            var notStartedAssignments = allAssignments.Count(a => a.Status == "NotStarted");
            var completionRate = totalAssignments > 0 ? (completedAssignments * 100 / totalAssignments) : 0;

            // Category analysis
            var categoryStats = _context.LearningAssignments
                .Include(a => a.Learning)
                .GroupBy(a => a.Learning.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    Total = g.Count(),
                    Completed = g.Count(a => a.Status == "Completed"),
                    CompletionRate = g.Count() > 0 ? (g.Count(a => a.Status == "Completed") * 100 / g.Count()) : 0
                })
                .ToList();

            ViewBag.TotalUsers = users.Count;
            ViewBag.TotalEmployees = employees.Count;
            ViewBag.TotalManagers = managers.Count;
            ViewBag.TotalHR = hrUsers.Count;
            ViewBag.Users = users;
            ViewBag.Employees = employees;

            // Metrics
            ViewBag.TotalAssignments = totalAssignments;
            ViewBag.CompletedAssignments = completedAssignments;
            ViewBag.InProgressAssignments = inProgressAssignments;
            ViewBag.NotStartedAssignments = notStartedAssignments;
            ViewBag.CompletionRate = completionRate;

            // All assignments for table
            ViewBag.AllAssignments = allAssignments.OrderByDescending(a => a.AssignedDate).Take(50).ToList();

            // Category stats
            ViewBag.CategoryStats = categoryStats;

            // Get all learnings for assignment
            ViewBag.AllLearnings = _context.Learnings.ToList();

            return View();
        }

        // User Management - List all users
        public IActionResult ManageUsers()
        {
            var users = _context.Users
                .Include(u => u.Manager)
                .OrderBy(u => u.Role)
                .ThenBy(u => u.UserName)
                .ToList();

            return View(users);
        }

        // Create User - GET
        [HttpGet]
        public IActionResult CreateUser()
        {
            ViewBag.Managers = _context.Users.Where(u => u.Role == "Manager").ToList();
            return View();
        }

        // Create User - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(ApplicationUser model, string password)
        {
            if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Username and password are required");
                ViewBag.Managers = _context.Users.Where(u => u.Role == "Manager").ToList();
                return View(model);
            }

            // Check if username already exists
            if (_context.Users.Any(u => u.UserName == model.UserName))
            {
                ModelState.AddModelError("UserName", "Username already exists");
                ViewBag.Managers = _context.Users.Where(u => u.Role == "Manager").ToList();
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                FullName = model.FullName,
                Email = model.Email,
                Role = model.Role,
                ManagerId = model.ManagerId,
                PasswordHash = _passwordHasher.HashPassword(null!, password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("HR {HR} created new user {User} with role {Role}", User.Identity?.Name, user.UserName, user.Role);
            TempData["SuccessMessage"] = $"User '{user.UserName}' created successfully!";

            return RedirectToAction(nameof(ManageUsers));
        }

        // Edit User - GET
        [HttpGet]
        public IActionResult EditUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            ViewBag.Managers = _context.Users.Where(u => u.Role == "Manager" && u.Id != id).ToList();
            return View(user);
        }

        // Edit User - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(int id, ApplicationUser model, string? newPassword)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            // Check username uniqueness
            if (_context.Users.Any(u => u.UserName == model.UserName && u.Id != id))
            {
                ModelState.AddModelError("UserName", "Username already exists");
                ViewBag.Managers = _context.Users.Where(u => u.Role == "Manager" && u.Id != id).ToList();
                return View(model);
            }

            user.UserName = model.UserName;
            user.FullName = model.FullName;
            user.Email = model.Email;
            user.Role = model.Role;
            user.ManagerId = model.ManagerId;

            // Update password if provided
            if (!string.IsNullOrEmpty(newPassword))
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            }

            _context.Update(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("HR {HR} updated user {User}", User.Identity?.Name, user.UserName);
            TempData["SuccessMessage"] = $"User '{user.UserName}' updated successfully!";

            return RedirectToAction(nameof(ManageUsers));
        }

        // Delete User - GET (Confirmation)
        [HttpGet]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users
                .Include(u => u.Manager)
                .Include(u => u.Assignments)
                .FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // Delete User - POST
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserConfirmed(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            var username = user.UserName;
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("HR {HR} deleted user {User}", User.Identity?.Name, username);
            TempData["SuccessMessage"] = $"User '{username}' deleted successfully!";

            return RedirectToAction(nameof(ManageUsers));
        }

        // View User Details
        [HttpGet]
        public IActionResult UserDetails(int id)
        {
            var user = _context.Users
                .Include(u => u.Manager)
                .Include(u => u.TeamMembers)
                .Include(u => u.Assignments)
                    .ThenInclude(a => a.Learning)
                .FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // Assign Role
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(int userId, string role)
        {
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return NotFound();
            }

            var oldRole = user.Role;
            user.Role = role;

            // If changing from Manager, unassign team members
            if (oldRole == "Manager" && role != "Manager")
            {
                var teamMembers = _context.Users.Where(u => u.ManagerId == userId).ToList();
                foreach (var member in teamMembers)
                {
                    member.ManagerId = null;
                }
            }

            _context.Update(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("HR {HR} changed role of {User} from {OldRole} to {NewRole}", 
                User.Identity?.Name, user.UserName, oldRole, role);

            TempData["SuccessMessage"] = $"Role changed from '{oldRole}' to '{role}' for user '{user.UserName}'";

            return RedirectToAction(nameof(ManageUsers));
        }

        // Reset Password
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(int userId, string newPassword)
        {
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return NotFound();
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            _context.Update(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("HR {HR} reset password for user {User}", User.Identity?.Name, user.UserName);
            TempData["SuccessMessage"] = $"Password reset successfully for user '{user.UserName}'";

            return RedirectToAction(nameof(EditUser), new { id = userId });
        }

        // HR Assign Learning to Employee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignLearningToEmployee(int employeeId, List<int> learningIds, DateTime? dueDate)
        {
            if (learningIds == null || !learningIds.Any())
            {
                TempData["ErrorMessage"] = "Please select at least one learning to assign.";
                return RedirectToAction(nameof(Index));
            }

            var employee = _context.Users.FirstOrDefault(u => u.Id == employeeId && u.Role == "Employee");
            if (employee == null)
            {
                TempData["ErrorMessage"] = "Employee not found.";
                return RedirectToAction(nameof(Index));
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
                    .FirstOrDefault(a => a.UserId == employeeId && a.LearningId == learningId);

                if (existingAssignment == null)
                {
                    var assignment = new LearningAssignment
                    {
                        UserId = employeeId,
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

                var message = $"Successfully assigned {assignedCount} learning(s) to {employee.UserName}.";
                if (skippedCount > 0)
                {
                    message += $" {skippedCount} learning(s) were already assigned.";
                }

                TempData["SuccessMessage"] = message;
                _logger.LogInformation("HR {HR} assigned {Count} learnings to employee {Employee}", 
                    User.Identity?.Name, assignedCount, employee.UserName);
            }
            else
            {
                TempData["InfoMessage"] = "All selected learnings were already assigned to this employee.";
            }

            return RedirectToAction(nameof(Index));
        }

        // Seed Sample Data (for demo/testing)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SeedSampleData()
        {
            try
            {
                // Call the data initializer
                LearningDataInitializer.InitializeLearnings(_context, _passwordHasher);

                TempData["SuccessMessage"] = "Sample data seeded successfully! Users, learnings, and assignments have been created.";
                _logger.LogInformation("HR {HR} triggered sample data seeding", User.Identity?.Name);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error seeding data: {ex.Message}";
                _logger.LogError(ex, "Error seeding sample data");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}