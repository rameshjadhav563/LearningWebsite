using LearningWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LearningWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Home page accessed");
            return View();
        }

        public IActionResult Privacy()
        {
            _logger.LogInformation("Privacy page accessed");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var errorId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            _logger.LogError("Error page displayed with RequestId: {RequestId}", errorId);
            return View(new ErrorViewModel { RequestId = errorId });
        }
    }
}
