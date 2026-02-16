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

        public IActionResult Index(int pageNumber = 1, int pageSize = 10)
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

            // Get all assignments for the team with learning details
            var teamMemberIds = teamMembers.Select(t => t.Id).ToList();
            var allAssignmentsQuery = _context.LearningAssignments
                .Include(a => a.User)
                .Include(a => a.Learning)
                .Where(a => teamMemberIds.Contains(a.UserId))
                .OrderByDescending(a => a.AssignedDate);

            var allAssignments = allAssignmentsQuery.ToList();
            var today = DateTime.Now.Date;

            // Calculate quick metrics
            var totalAssignments = allAssignments.Count;
            var completedAssignments = allAssignments.Count(a => a.Status == "Completed");
            var inProgressAssignments = allAssignments.Count(a => a.Status == "InProgress");
            var overdueAssignments = allAssignments.Count(a => a.DueDate < today && a.Status != "Completed");

            // Create paginated list
            var paginatedAssignments = PaginatedList<LearningAssignment>.Create(allAssignments, pageNumber, pageSize);

            // Calculate member statistics for team members table
            var memberStats = teamMembers.Select(m => new
            {
                MemberId = m.Id,
                TotalAssignments = allAssignments.Count(a => a.UserId == m.Id),
                CompletedAssignments = allAssignments.Count(a => a.UserId == m.Id && a.Status == "Completed"),
                OverdueAssignments = allAssignments.Count(a => a.UserId == m.Id && a.DueDate < today && a.Status != "Completed")
            }).ToDictionary(x => x.MemberId);

            ViewBag.Manager = manager;
            ViewBag.TeamMembers = teamMembers;
            ViewBag.TeamMemberCount = teamMembers.Count;
            ViewBag.AllAssignments = paginatedAssignments;
            ViewBag.TotalAssignments = totalAssignments;
            ViewBag.CompletedAssignments = completedAssignments;
            ViewBag.InProgressAssignments = inProgressAssignments;
            ViewBag.OverdueAssignments = overdueAssignments;
            ViewBag.MemberStats = memberStats;

            return View();
        }

        [HttpGet]
        public IActionResult TeamMemberDetail(int id, int pageNumber = 1, int pageSize = 10)
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

            // Paginate assignments
            var allAssignments = teamMember.Assignments?.OrderBy(a => a.DueDate).ToList() ?? new List<LearningAssignment>();
            var paginatedAssignments = PaginatedList<LearningAssignment>.Create(allAssignments, pageNumber, pageSize);

            ViewBag.TeamMember = teamMember;
            ViewBag.Assignments = paginatedAssignments;

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

        // Team Learning Metrics Dashboard
        [HttpGet]
        public IActionResult TeamMetrics(int teamMemberPage = 1, int categoryPage = 1, int activityPage = 1, int pageSize = 10)
        {
            _logger.LogInformation("Manager viewing team learning metrics");
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var manager = _context.Users.FirstOrDefault(u => u.UserName == userName);

            if (manager == null)
            {
                return NotFound();
            }

            // Get all team members
            var teamMemberIds = _context.Users
                .Where(u => u.ManagerId == manager.Id && u.Role == "Employee")
                .Select(u => u.Id)
                .ToList();

            // Get all assignments for team members
            var allAssignments = _context.LearningAssignments
                .Include(a => a.User)
                .Include(a => a.Learning)
                .Where(a => teamMemberIds.Contains(a.UserId))
                .ToList();

            var today = DateTime.Now.Date;

            // Calculate overall metrics
            var viewModel = new TeamMetricsViewModel
            {
                TotalTeamMembers = teamMemberIds.Count,
                TotalAssignments = allAssignments.Count,
                CompletedAssignments = allAssignments.Count(a => a.Status == "Completed"),
                InProgressAssignments = allAssignments.Count(a => a.Status == "InProgress"),
                NotStartedAssignments = allAssignments.Count(a => a.Status == "NotStarted"),
                OverdueAssignments = allAssignments.Count(a => a.DueDate < today && a.Status != "Completed"),
                TeamCompletionRate = allAssignments.Any()
                    ? Math.Round((double)allAssignments.Count(a => a.Status == "Completed") / allAssignments.Count * 100, 2)
                    : 0,
                AverageProgressPercentage = allAssignments.Any()
                    ? Math.Round(allAssignments.Average(a => a.ProgressPercentage ?? 0), 2)
                    : 0
            };

            // Calculate individual team member metrics
            var teamMembers = _context.Users
                .Where(u => teamMemberIds.Contains(u.Id))
                .ToList();

            var teamMemberMetrics = teamMembers
                .Select(u => new TeamMemberMetric
                {
                    UserId = u.Id,
                    UserName = u.UserName ?? "",
                    FullName = u.FullName ?? "",
                    TotalAssignments = allAssignments.Count(a => a.UserId == u.Id),
                    CompletedAssignments = allAssignments.Count(a => a.UserId == u.Id && a.Status == "Completed"),
                    InProgressAssignments = allAssignments.Count(a => a.UserId == u.Id && a.Status == "InProgress"),
                    NotStartedAssignments = allAssignments.Count(a => a.UserId == u.Id && a.Status == "NotStarted"),
                    OverdueAssignments = allAssignments.Count(a => a.UserId == u.Id && a.DueDate < today && a.Status != "Completed"),
                    CompletionRate = allAssignments.Any(a => a.UserId == u.Id)
                        ? Math.Round((double)allAssignments.Count(a => a.UserId == u.Id && a.Status == "Completed") /
                            allAssignments.Count(a => a.UserId == u.Id) * 100, 2)
                        : 0,
                    AverageProgress = allAssignments.Any(a => a.UserId == u.Id)
                        ? Math.Round(allAssignments.Where(a => a.UserId == u.Id).Average(a => a.ProgressPercentage ?? 0), 2)
                        : 0
                })
                .OrderByDescending(m => m.CompletionRate)
                .ToList();

            viewModel.TeamMemberMetrics = PaginatedList<TeamMemberMetric>.Create(teamMemberMetrics, teamMemberPage, pageSize);

            // Calculate category metrics
            var categoryMetrics = allAssignments
                .GroupBy(a => a.Learning?.Category ?? "Uncategorized")
                .Select(g => new CategoryMetric
                {
                    Category = g.Key,
                    TotalAssignments = g.Count(),
                    CompletedAssignments = g.Count(a => a.Status == "Completed"),
                    CompletionRate = Math.Round((double)g.Count(a => a.Status == "Completed") / g.Count() * 100, 2)
                })
                .OrderByDescending(c => c.CompletionRate)
                .ToList();

            viewModel.CategoryMetrics = PaginatedList<CategoryMetric>.Create(categoryMetrics, categoryPage, pageSize);

            // Recent activities
            var recentActivities = allAssignments
                .OrderByDescending(a => a.CompletedDate ?? a.AssignedDate)
                .Select(a => new RecentActivity
                {
                    UserName = a.User?.UserName ?? "",
                    LearningTitle = a.Learning?.Title ?? "",
                    Status = a.Status,
                    ActivityDate = a.CompletedDate ?? a.AssignedDate,
                    ActivityType = a.Status == "Completed" ? "Completed" :
                                   a.Status == "InProgress" ? "Started" : "Assigned"
                })
                .ToList();

            viewModel.RecentActivities = PaginatedList<RecentActivity>.Create(recentActivities, activityPage, pageSize);

            _logger.LogInformation("Manager {Manager} viewed team metrics. Team size: {TeamSize}, Total assignments: {Assignments}",
                manager.UserName, viewModel.TotalTeamMembers, viewModel.TotalAssignments);

            return View(viewModel);
        }
    }
}