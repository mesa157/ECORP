using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UnifiedFrontend.Models;
using Stripe;
using Microsoft.Extensions.Logging;
using UnifiedFrontend.Services;
using UnifiedFrontend.Services.ProductCatagory;

namespace UnifiedFrontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProductCatalogApi _apiService;

        public HomeController(ILogger<HomeController> logger, ProductCatalogApi apiService)
        {
            _logger = logger;
            _apiService = apiService;
        }

        // Unified Index action that includes navigation for all services
        public async Task<IActionResult> Index()
        {
            // Load sample data (e.g., featured products or categories)
            var categories = await _apiService.GetCategoriesAsync();
            ViewBag.FeaturedCategories = categories?.Take(3); // Show only a few categories for the homepage

            return View();
        }

        // Privacy action (keep as-is for the unified front end)
        public IActionResult Privacy()
        {
            return View();
        }

        // Error handling
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
