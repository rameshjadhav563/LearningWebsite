using LearningWebsite.Data;
using LearningWebsite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LearningWebsite.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(AppDbContext context, ILogger<DashboardController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get dashboard data for employees
        /// </summary>
        [HttpGet("employee")]
        [Authorize(Policy = "EmployeeOnly")]
        public async Task<IActionResult> GetEmployeeDashboard()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized();

                var assignments = await _context.LearningAssignments
                    .Include(la => la.Learning)
                    .Where(la => la.UserId == userId)
                    .OrderByDescending(la => la.AssignedDate)
                    .ToListAsync();

                // Get certificate count
                var certificatesCount = await _context.Certificates
                    .CountAsync(c => c.UserId == userId);

                var dashboardData = new
                {
                    TotalAssignments = assignments.Count,
                    Completed = assignments.Count(a => a.Status == "Completed"),
                    InProgress = assignments.Count(a => a.Status == "InProgress"),
                    NotStarted = assignments.Count(a => a.Status == "NotStarted"),
                    CertificatesCount = certificatesCount,
                    Assignments = assignments.Select(a => new
                    {
                        a.Id,
                        a.LearningId,
                        a.Learning!.Title,
                        a.Learning.Category,
                        a.Status,
                        a.AssignedDate,
                        a.DueDate,
                        a.ProgressPercentage,
                        a.CompletedDate,
                        DaysUntilDue = (a.DueDate - DateTime.Now).Days
                    }).ToList()
                };

                return Ok(dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching employee dashboard");
                return StatusCode(500, "An error occurred while fetching dashboard data");
            }
        }

        /// <summary>
        /// Get dashboard data for managers
        /// </summary>
        [HttpGet("manager")]
        [Authorize(Policy = "ManagerOnly")]
        public async Task<IActionResult> GetManagerDashboard()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized();

                // Get current user to identify their team
                var manager = await _context.Users.FindAsync(userId);
                if (manager == null)
                    return NotFound();

                // For this example, we'll get all team members (you can adjust based on your team structure)
                var teamMembers = await _context.Users
                    .Where(u => u.Role == "Employee")
                    .Select(u => u.Id)
                    .ToListAsync();

                var teamAssignments = await _context.LearningAssignments
                    .Include(la => la.User)
                    .Include(la => la.Learning)
                    .Where(la => teamMembers.Contains(la.UserId))
                    .OrderByDescending(la => la.AssignedDate)
                    .ToListAsync();

                var teamMetrics = new
                {
                    TotalTeamMembers = teamMembers.Count,
                    TotalAssignments = teamAssignments.Count,
                    CompletedAssignments = teamAssignments.Count(a => a.Status == "Completed"),
                    InProgressAssignments = teamAssignments.Count(a => a.Status == "InProgress"),
                    NotStartedAssignments = teamAssignments.Count(a => a.Status == "NotStarted"),
                    CompletionRate = teamAssignments.Count > 0 
                        ? Math.Round((decimal)teamAssignments.Count(a => a.Status == "Completed") / teamAssignments.Count * 100, 2)
                        : 0,
                    TeamAssignments = teamAssignments
                        .GroupBy(la => la.UserId)
                        .Select(g => new
                        {
                            UserName = g.First().User!.UserName,
                            TotalAssigned = g.Count(),
                            Completed = g.Count(a => a.Status == "Completed"),
                            InProgress = g.Count(a => a.Status == "InProgress"),
                            NotStarted = g.Count(a => a.Status == "NotStarted"),
                            Assignments = g.Select(a => new
                            {
                                a.Id,
                                a.Learning!.Title,
                                a.Status,
                                a.AssignedDate,
                                a.DueDate,
                                a.ProgressPercentage
                            }).ToList()
                        }).ToList()
                };

                return Ok(teamMetrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching manager dashboard");
                return StatusCode(500, "An error occurred while fetching dashboard data");
            }
        }

        /// <summary>
        /// Get dashboard data for HR
        /// </summary>
        [HttpGet("hr")]
        [Authorize(Policy = "HROnly")]
        public async Task<IActionResult> GetHRDashboard()
        {
            try
            {
                var allUsers = await _context.Users.ToListAsync();
                var allAssignments = await _context.LearningAssignments
                    .Include(la => la.User)
                    .Include(la => la.Learning)
                    .ToListAsync();

                var hrMetrics = new
                {
                    TotalEmployees = allUsers.Count(u => u.Role == "Employee"),
                    TotalManagers = allUsers.Count(u => u.Role == "Manager"),
                    TotalAssignments = allAssignments.Count,
                    CompletedAssignments = allAssignments.Count(a => a.Status == "Completed"),
                    InProgressAssignments = allAssignments.Count(a => a.Status == "InProgress"),
                    NotStartedAssignments = allAssignments.Count(a => a.Status == "NotStarted"),
                    OverallCompletionRate = allAssignments.Count > 0
                        ? Math.Round((decimal)allAssignments.Count(a => a.Status == "Completed") / allAssignments.Count * 100, 2)
                        : 0,
                    CompletionByCategory = allAssignments
                        .GroupBy(a => a.Learning!.Category)
                        .Select(g => new
                        {
                            Category = g.Key,
                            Total = g.Count(),
                            Completed = g.Count(a => a.Status == "Completed"),
                            CompletionRate = Math.Round((decimal)g.Count(a => a.Status == "Completed") / g.Count() * 100, 2)
                        }).ToList(),
                    EmployeeProgress = allUsers
                        .Where(u => u.Role == "Employee")
                        .Select(u => new
                        {
                            u.UserName,
                            TotalAssignments = allAssignments.Count(a => a.UserId == u.Id),
                            Completed = allAssignments.Count(a => a.UserId == u.Id && a.Status == "Completed"),
                            CompletionRate = allAssignments.Where(a => a.UserId == u.Id).Count() > 0
                                ? Math.Round((decimal)allAssignments.Count(a => a.UserId == u.Id && a.Status == "Completed") / allAssignments.Count(a => a.UserId == u.Id) * 100, 2)
                                : 0
                        }).ToList()
                };

                return Ok(hrMetrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching HR dashboard");
                return StatusCode(500, "An error occurred while fetching dashboard data");
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }
            return 0;
        }
    }
}
