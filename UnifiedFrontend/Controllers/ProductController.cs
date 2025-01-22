using Microsoft.AspNetCore.Mvc;
using Stripe;
using UnifiedFrontend.Models.ProductCatalogModel;
using UnifiedFrontend.Services.ProductCatagory;

namespace UnifiedFrontend.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductCatalogApi _apiService;

        public ProductsController(ProductCatalogApi apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(Guid? categoryId, string search, int page = 1)
        {
            const int PageSize = 10;

            List<ProductViewModel> products;
            if (string.IsNullOrEmpty(search) && !categoryId.HasValue)
            {
                products = await _apiService.GetAllProductsAsync();
            }
            else
            {
                products = await _apiService.GetProductsAsync(categoryId, search, page, PageSize);
            }

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = PageSize;
            ViewBag.CategoryId = categoryId;
            ViewBag.Search = search;

            return View(products);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var product = await _apiService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return View(product);
        }
    }
}
