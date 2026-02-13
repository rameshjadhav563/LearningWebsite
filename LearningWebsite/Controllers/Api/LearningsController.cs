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
    public class LearningsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<LearningsController> _logger;

        public LearningsController(AppDbContext context, ILogger<LearningsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all available learnings
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Learning>>> GetLearnings()
        {
            try
            {
                var learnings = await _context.Learnings.ToListAsync();
                return Ok(learnings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching learnings");
                return StatusCode(500, "An error occurred while fetching learnings");
            }
        }

        /// <summary>
        /// Get a specific learning by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Learning>> GetLearning(int id)
        {
            try
            {
                var learning = await _context.Learnings.FindAsync(id);
                if (learning == null)
                    return NotFound();

                return Ok(learning);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching learning");
                return StatusCode(500, "An error occurred");
            }
        }

        /// <summary>
        /// Create a new learning (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "HROnly")]
        public async Task<ActionResult<Learning>> CreateLearning(Learning learning)
        {
            try
            {
                _context.Learnings.Add(learning);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetLearning), new { id = learning.Id }, learning);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating learning");
                return StatusCode(500, "An error occurred while creating learning");
            }
        }

        /// <summary>
        /// Update learning (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "HROnly")]
        public async Task<IActionResult> UpdateLearning(int id, Learning learning)
        {
            if (id != learning.Id)
                return BadRequest("ID mismatch");

            try
            {
                _context.Entry(learning).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating learning");
                return StatusCode(500, "An error occurred while updating learning");
            }
        }

        /// <summary>
        /// Delete learning (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "HROnly")]
        public async Task<IActionResult> DeleteLearning(int id)
        {
            try
            {
                var learning = await _context.Learnings.FindAsync(id);
                if (learning == null)
                    return NotFound();

                _context.Learnings.Remove(learning);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting learning");
                return StatusCode(500, "An error occurred while deleting learning");
            }
        }
    }
}
