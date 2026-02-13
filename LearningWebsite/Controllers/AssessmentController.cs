using LearningWebsite.Data;
using LearningWebsite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LearningWebsite.Controllers
{
    [Authorize(Policy = "EmployeeOnly")]
    public class AssessmentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AssessmentController> _logger;

        public AssessmentController(AppDbContext context, ILogger<AssessmentController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Training Page - View learning material
        public async Task<IActionResult> Training(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                _logger.LogInformation("User {UserId} ({UserName}) accessing training for learning {LearningId}", 
                    userId, User.Identity?.Name, id);

                var assignment = await _context.LearningAssignments
                    .Include(la => la.Learning)
                    .FirstOrDefaultAsync(la => la.UserId == userId && la.LearningId == id);

                if (assignment == null)
                {
                    _logger.LogWarning("Learning assignment not found for User {UserId}, Learning {LearningId}", 
                        userId, id);
                    return NotFound("Learning assignment not found.");
                }

                // Check if assessment is already completed
                var existingResult = await _context.AssessmentResults
                    .FirstOrDefaultAsync(ar => ar.UserId == userId && ar.LearningId == id);

                ViewBag.AssessmentCompleted = existingResult != null;
                ViewBag.AssessmentPassed = existingResult?.Passed ?? false;

                _logger.LogInformation("Training page loaded successfully for User {UserId}, Learning '{LearningTitle}'", 
                    userId, assignment.Learning?.Title);

                return View(assignment.Learning);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading training page for learning {LearningId}", id);
                return StatusCode(500, "An error occurred while loading the training page.");
            }
        }

        // Assessment Page - Display random questions
        public async Task<IActionResult> TakeAssessment(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                _logger.LogInformation("User {UserId} ({UserName}) starting assessment for learning {LearningId}", 
                    userId, User.Identity?.Name, id);

                // Check if user has this learning assigned
                var assignment = await _context.LearningAssignments
                    .Include(la => la.Learning)
                    .FirstOrDefaultAsync(la => la.UserId == userId && la.LearningId == id);

                if (assignment == null)
                {
                    _logger.LogWarning("Learning assignment not found for User {UserId}, Learning {LearningId}", 
                        userId, id);
                    return NotFound("Learning assignment not found.");
                }

                // Get random questions for this learning
                var allQuestions = await _context.Questions
                    .Where(q => q.LearningId == id)
                    .ToListAsync();

                if (!allQuestions.Any())
                {
                    _logger.LogWarning("No questions available for learning {LearningId}", id);
                    TempData["Error"] = "No questions available for this assessment.";
                    return RedirectToAction("Training", new { id });
                }

                // Select random questions (maximum 10)
                var random = new Random();
                var selectedQuestions = allQuestions
                    .OrderBy(q => random.Next())
                    .Take(Math.Min(10, allQuestions.Count))
                    .ToList();

                _logger.LogInformation("Generated {QuestionCount} random questions for User {UserId}, Learning '{LearningTitle}'", 
                    selectedQuestions.Count, userId, assignment.Learning?.Title);

                var viewModel = new AssessmentViewModel
                {
                    LearningId = id,
                    LearningTitle = assignment.Learning!.Title,
                    Questions = selectedQuestions.Select(q => new QuestionViewModel
                    {
                        QuestionId = q.Id,
                        QuestionText = q.QuestionText,
                        OptionA = q.OptionA,
                        OptionB = q.OptionB,
                        OptionC = q.OptionC,
                        OptionD = q.OptionD
                    }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading assessment for learning {LearningId}", id);
                return StatusCode(500, "An error occurred while loading the assessment.");
            }
        }

        // Submit Assessment
        [HttpPost]
        public async Task<IActionResult> SubmitAssessment(AssessmentSubmissionModel submission)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                _logger.LogInformation("User {UserId} ({UserName}) submitting assessment for learning {LearningId}", 
                    userId, User.Identity?.Name, submission.LearningId);

                // Get all questions with correct answers
                var questionIds = submission.Answers.Keys.ToList();
                var questions = await _context.Questions
                    .Where(q => questionIds.Contains(q.Id))
                    .ToListAsync();

                // Calculate score
                int correctAnswers = 0;
                var questionResults = new List<QuestionResultViewModel>();

                foreach (var question in questions)
                {
                    var userAnswer = submission.Answers.ContainsKey(question.Id) 
                        ? submission.Answers[question.Id] 
                        : "";
                    var isCorrect = userAnswer.Equals(question.CorrectAnswer, StringComparison.OrdinalIgnoreCase);

                    if (isCorrect)
                    {
                        correctAnswers++;
                    }

                    questionResults.Add(new QuestionResultViewModel
                    {
                        QuestionText = question.QuestionText,
                        YourAnswer = GetAnswerText(question, userAnswer),
                        CorrectAnswer = GetAnswerText(question, question.CorrectAnswer),
                        IsCorrect = isCorrect
                    });
                }

                int totalQuestions = questions.Count;
                decimal score = totalQuestions > 0 ? (decimal)correctAnswers / totalQuestions * 100 : 0;
                bool passed = score >= 70;

                // Get difficulty level from first question (assuming all questions have same level)
                string difficultyLevel = questions.FirstOrDefault()?.DifficultyLevel ?? "Beginner";

                _logger.LogInformation("Assessment completed: User {UserId}, Learning {LearningId}, Score {Score}%, Passed: {Passed}", 
                    userId, submission.LearningId, score, passed);

                // Save assessment result
                var assessmentResult = new AssessmentResult
                {
                    UserId = userId,
                    LearningId = submission.LearningId,
                    CompletedDate = DateTime.Now,
                    TotalQuestions = totalQuestions,
                    CorrectAnswers = correctAnswers,
                    Score = score,
                    Passed = passed,
                    DifficultyLevel = difficultyLevel
                };

                _context.AssessmentResults.Add(assessmentResult);
                await _context.SaveChangesAsync(); // Save first to get the AssessmentResult ID

                // Save detailed answer information
                foreach (var question in questions)
                {
                    var userAnswer = submission.Answers.ContainsKey(question.Id) 
                        ? submission.Answers[question.Id] 
                        : "";

                    var answerDetail = new AssessmentAnswerDetail
                    {
                        AssessmentResultId = assessmentResult.Id,
                        QuestionId = question.Id,
                        QuestionText = question.QuestionText,
                        UserAnswer = GetAnswerText(question, userAnswer),
                        CorrectAnswer = GetAnswerText(question, question.CorrectAnswer),
                        IsCorrect = userAnswer.Equals(question.CorrectAnswer, StringComparison.OrdinalIgnoreCase)
                    };

                    _context.AssessmentAnswerDetails.Add(answerDetail);
                }

                // Get learning title (needed for certificate)
                var learning = await _context.Learnings.FindAsync(submission.LearningId);

                // Update assignment status if passed
                if (passed)
                {
                    var assignment = await _context.LearningAssignments
                        .FirstOrDefaultAsync(la => la.UserId == userId && la.LearningId == submission.LearningId);

                    if (assignment != null)
                    {
                        assignment.Status = "Completed";
                        assignment.ProgressPercentage = 100;
                        assignment.CompletedDate = DateTime.Now;

                        _logger.LogInformation("Assignment auto-completed for User {UserId}, Learning {LearningId}", 
                            userId, submission.LearningId);
                    }

                    // Generate certificate for passed assessment
                    var user = await _context.Users.FindAsync(userId);
                    var certificateNumber = GenerateCertificateNumber(userId, submission.LearningId);

                    var certificate = new Certificate
                    {
                        UserId = userId,
                        LearningId = submission.LearningId,
                        AssessmentResultId = assessmentResult.Id,
                        CertificateNumber = certificateNumber,
                        IssuedDate = DateTime.Now,
                        Score = score,
                        Title = $"{learning?.Title} - {difficultyLevel} Level",
                        DifficultyLevel = difficultyLevel,
                        LearningTitle = learning?.Title ?? "Assessment",
                        EmployeeName = user?.FullName ?? User.Identity?.Name ?? "Employee",
                        Description = $"Successfully completed {learning?.Title} with a score of {score:F1}%"
                    };

                    _context.Certificates.Add(certificate);
                    _logger.LogInformation("Certificate {CertificateNumber} generated for User {UserId}, Learning {LearningId}",
                        certificateNumber, userId, submission.LearningId);
                }

                await _context.SaveChangesAsync();

                var resultViewModel = new AssessmentResultViewModel
                {
                    LearningTitle = learning?.Title ?? "Assessment",
                    TotalQuestions = totalQuestions,
                    CorrectAnswers = correctAnswers,
                    Score = score,
                    Passed = passed,
                    CompletedDate = assessmentResult.CompletedDate,
                    QuestionResults = questionResults
                };

                return View("AssessmentResult", resultViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting assessment for learning {LearningId}", submission.LearningId);
                return StatusCode(500, "An error occurred while submitting the assessment.");
            }
        }

        // Helper method to get answer text
        private string GetAnswerText(Question question, string answerOption)
        {
            return answerOption.ToUpper() switch
            {
                "A" => question.OptionA,
                "B" => question.OptionB,
                "C" => question.OptionC,
                "D" => question.OptionD,
                _ => "Not Answered"
            };
        }

        // Helper method to generate unique certificate number
        // Format: CERT-YYYYMMDD-XXXX-YYYY
        private string GenerateCertificateNumber(int userId, int learningId)
        {
            var dateString = DateTime.Now.ToString("yyyyMMdd");
            var userPart = userId.ToString("D4");
            var learningPart = learningId.ToString("D4");
            return $"CERT-{dateString}-{userPart}-{learningPart}";
        }

        // View assessment history
        public async Task<IActionResult> AssessmentHistory()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                _logger.LogInformation("User {UserId} ({UserName}) viewing assessment history", 
                    userId, User.Identity?.Name);

                // Get all learning IDs that are assigned to this user
                var assignedLearningIds = await _context.LearningAssignments
                    .Where(la => la.UserId == userId)
                    .Select(la => la.LearningId)
                    .ToListAsync();

                // Get all assessment results for assigned learnings only
                var allResults = await _context.AssessmentResults
                    .Include(ar => ar.Learning)
                    .Where(ar => ar.UserId == userId && assignedLearningIds.Contains(ar.LearningId))
                    .OrderByDescending(ar => ar.CompletedDate)
                    .ToListAsync();

                // Group by LearningId and take the latest attempt (in memory)
                var results = allResults
                    .GroupBy(ar => ar.LearningId)
                    .Select(g => g.First()) // First is already the latest due to OrderBy above
                    .ToList();

                _logger.LogInformation("Retrieved {Count} unique assessment results for User {UserId}", 
                    results.Count, userId);

                return View(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading assessment history");
                return StatusCode(500, "An error occurred while loading the assessment history.");
            }
        }

        // View detailed assessment review
        public async Task<IActionResult> ReviewAssessment(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var assessmentResult = await _context.AssessmentResults
                    .Include(ar => ar.Learning)
                    .Include(ar => ar.AnswerDetails)
                    .FirstOrDefaultAsync(ar => ar.Id == id && ar.UserId == userId);

                if (assessmentResult == null)
                {
                    return NotFound("Assessment result not found.");
                }

                // Check if certificate exists for this assessment
                var certificate = await _context.Certificates
                    .FirstOrDefaultAsync(c => c.AssessmentResultId == id && c.UserId == userId);

                var viewModel = new AssessmentResultViewModel
                {
                    LearningTitle = assessmentResult.Learning?.Title ?? "Assessment",
                    TotalQuestions = assessmentResult.TotalQuestions,
                    CorrectAnswers = assessmentResult.CorrectAnswers,
                    Score = assessmentResult.Score,
                    Passed = assessmentResult.Passed,
                    CompletedDate = assessmentResult.CompletedDate,
                    QuestionResults = assessmentResult.AnswerDetails.Select(ad => new QuestionResultViewModel
                    {
                        QuestionText = ad.QuestionText,
                        YourAnswer = ad.UserAnswer,
                        CorrectAnswer = ad.CorrectAnswer,
                        IsCorrect = ad.IsCorrect
                    }).ToList()
                };

                // Pass certificate information to the view
                ViewBag.CertificateId = certificate?.Id;
                ViewBag.HasCertificate = certificate != null;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading assessment review for result {ResultId}", id);
                return StatusCode(500, "An error occurred while loading the assessment review.");
            }
        }
    }
}
