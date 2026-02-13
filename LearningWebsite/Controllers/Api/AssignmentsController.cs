using LearningWebsite.Data;
using LearningWebsite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearningWebsite.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AssignmentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AssignmentsController> _logger;

        public AssignmentsController(AppDbContext context, ILogger<AssignmentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Create a new assignment (Manager/HR only)
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<LearningAssignment>> CreateAssignment(CreateAssignmentRequest request)
        {
            try
            {
                var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                if (userRole != "Manager" && userRole != "HR")
                    return Forbid();

                var learning = await _context.Learnings.FindAsync(request.LearningId);
                if (learning == null)
                    return NotFound("Learning not found");

                var user = await _context.Users.FindAsync(request.UserId);
                if (user == null)
                    return NotFound("User not found");

                var assignment = new LearningAssignment
                {
                    UserId = request.UserId,
                    LearningId = request.LearningId,
                    AssignedDate = DateTime.Now,
                    DueDate = request.DueDate,
                    Status = "NotStarted",
                    ProgressPercentage = 0
                };

                _context.LearningAssignments.Add(assignment);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetAssignment), new { id = assignment.Id }, assignment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating assignment");
                return StatusCode(500, "An error occurred while creating assignment");
            }
        }

        /// <summary>
        /// Get a specific assignment
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<LearningAssignment>> GetAssignment(int id)
        {
            try
            {
                var assignment = await _context.LearningAssignments
                    .Include(a => a.Learning)
                    .Include(a => a.User)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (assignment == null)
                    return NotFound();

                return Ok(assignment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching assignment");
                return StatusCode(500, "An error occurred");
            }
        }

        /// <summary>
        /// Update assignment status and progress
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAssignment(int id, UpdateAssignmentRequest request)
        {
            try
            {
                var assignment = await _context.LearningAssignments.FindAsync(id);
                if (assignment == null)
                    return NotFound();

                // Verify user has permission to update
                var userId = int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var uid) ? uid : 0;
                var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

                if (assignment.UserId != userId && userRole != "Manager" && userRole != "HR")
                    return Forbid();

                assignment.Status = request.Status ?? assignment.Status;
                assignment.ProgressPercentage = request.ProgressPercentage ?? assignment.ProgressPercentage;
                
                if (request.Status == "Completed" && assignment.CompletedDate == null)
                    assignment.CompletedDate = DateTime.Now;

                _context.LearningAssignments.Update(assignment);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating assignment");
                return StatusCode(500, "An error occurred while updating assignment");
            }
        }

        /// <summary>
        /// Delete an assignment (Manager/HR only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            try
            {
                var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                if (userRole != "Manager" && userRole != "HR")
                    return Forbid();

                var assignment = await _context.LearningAssignments.FindAsync(id);
                if (assignment == null)
                    return NotFound();

                _context.LearningAssignments.Remove(assignment);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting assignment");
                return StatusCode(500, "An error occurred while deleting assignment");
            }
        }
    }

    public class CreateAssignmentRequest
    {
        public int UserId { get; set; }
        public int LearningId { get; set; }
        public DateTime DueDate { get; set; }
    }

    public class UpdateAssignmentRequest
    {
        public string? Status { get; set; }
        public int? ProgressPercentage { get; set; }
    }
}
