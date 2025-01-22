using Microsoft.AspNetCore.Mvc;
using Stripe;
using UnifiedFrontend.Models.ProductCatalogModel;
using UnifiedFrontend.Services.ProductCatagory;

namespace UnifiedFrontend.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ProductCatalogApi _apiService;

        public CategoriesController(ProductCatalogApi apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _apiService.GetCategoriesAsync();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Call API to create category (to be implemented)
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
